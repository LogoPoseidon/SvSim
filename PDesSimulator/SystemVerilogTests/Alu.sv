`timescale 1ns / 1ps

module simple_alu (
    input  logic        clk,
    input  logic        rst_n,
    input  logic [3:0]  a,
    input  logic [3:0]  b,
    input  logic [1:0]  opcode,
    output logic [3:0]  out_comb,
    output logic [3:0]  out_reg,
    output logic        zero_flag
);

    always_comb begin
        case (opcode)
            2'b00:   out_comb = a + b;
            2'b01:   out_comb = a - b;
            2'b10:   out_comb = a & b;
            2'b11:   out_comb = a ^ b;
            default: out_comb = 4'b0000;
        endcase
    end

    always_ff @(posedge clk) begin
        if (!rst_n) begin
            out_reg <= 4'b0000;
        end else begin
            out_reg <= out_comb;
        end
    end

    assign zero_flag = (out_comb == 4'b0000);

endmodule