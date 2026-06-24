`timescale 1ns/1ps

module tb_simple_alu;

    logic        clk;
    logic        rst_n;
    logic [3:0]  a;
    logic [3:0]  b;
    logic [1:0]  opcode;

    logic [3:0]  out_comb;
    logic [3:0]  out_reg;
    logic        zero_flag;

    simple_alu uut (
        .clk(clk),
        .rst_n(rst_n),
        .a(a),
        .b(b),
        .opcode(opcode),
        .out_comb(out_comb),
        .out_reg(out_reg),
        .zero_flag(zero_flag)
    );

    always begin
        #10 clk = ~clk;
    end

    initial begin
        $dumpfile("out.vcd");
        $dumpvars(0, tb_simple_alu);
        clk = 0;
        rst_n = 0;
        a = 4'b0000;
        b = 4'b0000;
        opcode = 2'b00;

        #20;
        rst_n = 1;
        #10;

        $display("\n--- Test Case 1: Addition ---");
        a = 4'd5;
        b = 4'd3;
        opcode = 2'b00; 
        #20;

        $display("\n--- Test Case 2: Subtraction (Zero Check) ---");
        a = 4'd4;
        b = 4'd4;
        opcode = 2'b01; 
        #20;

        $display("\n--- Test Case 3: Bitwise AND ---");
        a = 4'b1100;
        b = 4'b1010;
        opcode = 2'b10; 
        #20;

        $display("\n--- Test Case 4: Bitwise XOR ---");
        a = 4'b1100;
        b = 4'b1010;
        opcode = 2'b11; 
        #20;

        $display("\nSimulation complete.");
        $finish;
    end

    initial begin
        $monitor("Time=%0t ns | rst_n=%b | opcode=%b | a=%b b=%b | out_comb=%b | out_reg=%b | zero=%b",
                 $time, rst_n, opcode, a, b, out_comb, out_reg, zero_flag);
    end

endmodule