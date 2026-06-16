package alu_pkg;
    typedef enum logic [2:0] { ADD=0, SUB=1, AND=2, OR=3, XOR=4 } op_t;

    typedef struct packed {
        logic [31:0] a;
        logic [31:0] b;
        op_t op;
    } tx_t;

    typedef union packed {
        logic [15:0] raw_val;
        struct packed {
            logic [7:0] msb;
            logic [7:0] lsb;
        } bytes;
    } payload_u;

    typedef struct {
        string desc;
        int sim_time;
        logic [31:0] result;
    } log_entry_t;

    import "DPI-C" context task alu_external_sync(input int duration, output int status);

    virtual class AluBaseTx;
        pure virtual function void display(string prefix = "TX");
        pure virtual protected function bit validate_operands();
        pure virtual task run_execution_flow();
    endclass

    class AluTransaction #(int W = 8) extends AluBaseTx;
        rand bit [W-1:0] op_a;
        rand bit [W-1:0] op_b;
        rand op_t        op_type;

        constraint c_op_dist {
            op_type dist { ADD := 40, SUB := 20, AND := 10, OR := 10, XOR := 20 };
        }

        extern virtual function void display(string prefix = "TX");
        extern virtual protected function bit validate_operands();
        extern virtual task run_execution_flow();
    endclass

    function void AluTransaction::display(string prefix = "TX");
        $display("[%s] Op: %s | A: %h | B: %h", prefix, op_type.name(), op_a, op_b);
    endfunction

    function bit AluTransaction::validate_operands();
        return (op_a !== 'x && op_b !== 'x);
    endfunction

    task AluTransaction::run_execution_flow();
        #5ns;
    endtask

endpackage

interface alu_if #(parameter W = 8) (input logic clk);
    logic rst_n;
    logic [W-1:0] a;
    logic [W-1:0] b;
    logic [W-1:0] result;
    logic [W-1:0] accum;
    alu_pkg::op_t op;

    clocking cb @(posedge clk);
        default input #1ns output #1ns;
        output a, b, op;
        input  result, accum;
    endclocking

    task automatic monitor_bus(output logic [W-1:0] val);
        @(posedge clk);
        val = result;
    endtask

    modport dut (input clk, rst_n, a, b, op, output result, accum);

    modport tb  (
        clocking cb, 
        output rst_n, 
        import task monitor_bus(output logic [W-1:0] val)
    );

    always_comb begin
        if (rst_n) begin
            a_unknown_inputs: assert (!$isunknown({a, b, op})) 
                else $warning("Interface %0d-bit has X/Z values in inputs", W);
        end
    end
endinterface

module alu_core #(parameter bit USE_XOR = 1) (
    alu_if.dut bus
);
    import alu_pkg::*;

    always_comb begin
        case (bus.op)
            ADD: bus.result = bus.a + bus.b;
            SUB: bus.result = bus.a - bus.b;
            AND: bus.result = bus.a & bus.b;
            OR:  bus.result = bus.a | bus.b;
            XOR: bus.result = USE_XOR ? (bus.a ^ bus.b) : '0;
            default: bus.result = '0;
        endcase
    end

    always_ff @(posedge bus.clk or negedge bus.rst_n) begin
        if (!bus.rst_n) begin
            bus.accum <= '0;
        end else begin
            bus.accum <= bus.accum + bus.result;
        end
    end

    property p_accum_reset;
        @(posedge bus.clk) !bus.rst_n |=> (bus.accum == '0);
    endproperty
    a_accum_reset: assert property (p_accum_reset);
endmodule

module alu_monitor #(parameter int W = 8, parameter int INST_ID = 0) (
    alu_if.dut bus
);
    always @(posedge bus.clk) begin
        if (bus.rst_n && (bus.op != alu_pkg::ADD || bus.result != '0)) begin
            $display("[MON Core %0d (W=%0d)] Op: %s | A: %h | B: %h | Res: %h | Acc: %h", 
                     INST_ID, W, bus.op.name(), bus.a, bus.b, bus.result, bus.accum);
        end
    end
endmodule

module top;
    import alu_pkg::*;

    localparam int NUM_CORES = 3;

    logic clk;
    initial begin
        clk = 0;
        forever #5 clk = ~clk;
    end

    generate
        for (genvar i = 0; i < NUM_CORES; i++) begin : alu_gen
            localparam int WIDTH = (i == 0) ? 8 : (i == 1) ? 16 : 32;
            localparam bit ENABLE_XOR = (i != 1);

            alu_if #(.W(WIDTH)) io(clk);

            alu_core #(.USE_XOR(ENABLE_XOR)) dut (.bus(io.dut));

            alu_monitor #(.W(WIDTH), .INST_ID(i)) mon (.bus(io.dut));
        end
    endgenerate

    tx_t stimulus_db[];
    op_t op_history[$];
    int op_counters[op_t];
    log_entry_t execution_log[$];

    covergroup alu_cov_g with function sample(op_t op, logic [31:0] a, logic [31:0] b);
        option.per_instance = 1;
        cp_op: coverpoint op;
        cp_a:  coverpoint a[7:0] {
            bins zero = {8'h00};
            bins low  = {[8'h01:8'h7F]};
            bins high = {[8'h80:8'hFF]};
        }
        cp_b:  coverpoint b[7:0] {
            bins zero = {8'h00};
            bins low  = {[8'h01:8'h7F]};
            bins high = {[8'h80:8'hFF]};
        }
    endgroup

    alu_cov_g cov_inst = new();

    // Watchdog Timer
    initial begin
        #500;
        $fatal(1, "[TIMEOUT] Simulation hung, exiting safety watch loop.");
    end

    generate
        for (genvar i = 0; i < NUM_CORES; i++) begin : driver_gen
            initial begin
                // Initialize resets
                alu_gen[i].io.rst_n = 0;
                alu_gen[i].io.a = '0;
                alu_gen[i].io.b = '0;
                alu_gen[i].io.op = ADD;
                
                #15;
                alu_gen[i].io.rst_n = 1;
                
                begin
                    automatic AluTransaction #(.W((i == 0) ? 8 : (i == 1) ? 16 : 32)) tx = new();
                    
                    repeat (5) begin
                        if (!tx.randomize()) begin
                            $error("Randomization failed for driver %0d", i);
                        end else begin
                            @(alu_gen[i].io.cb);
                            alu_gen[i].io.cb.op <= tx.op_type;
                            alu_gen[i].io.cb.a  <= tx.op_a;
                            alu_gen[i].io.cb.b  <= tx.op_b;
                            
                            tx.run_execution_flow();

                            cov_inst.sample(tx.op_type, tx.op_a, tx.op_b);
                        end
                    end
                end
            end
        end
    endgenerate

    initial begin
        payload_u union_data;
        $display("[SYSTEM] Commencing Multi-Core Parallel Execution Engine...");

        stimulus_db = new[3];
        stimulus_db[0] = '{a: 32'h05, b: 32'h0A, op: ADD};
        stimulus_db[1] = '{a: 32'h10, b: 32'h02, op: SUB};
        stimulus_db[2] = '{a: 32'hFF, b: 32'h0F, op: AND};

        foreach (stimulus_db[j]) begin
            op_counters[stimulus_db[j].op]++;
            op_history.push_back(stimulus_db[j].op);
        end

        union_data.raw_val = 16'h1234;
        $display("[UNION] Raw Value: %h (MSB: %h, LSB: %h)", 
                 union_data.raw_val, union_data.bytes.msb, union_data.bytes.lsb);

        #150;

        $display("\n=======================================================");
        $display("POST-RUN VERIFICATION SUMMARY");
        $display("=======================================================");
        $display("Active unique operations tracked globally in database: %0d", op_history.size());
        $display("Functional Coverage Achieved: %0.2f%%", cov_inst.get_inst_coverage());
        $display("=======================================================");
        
        $finish;
    end
endmodule