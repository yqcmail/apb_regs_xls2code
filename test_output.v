module my_reg_block (
    // APB Interface
    input           PCLK,
    input           PRESETn,
    input           PSEL,
    input           PENABLE,
    input   [31:0]  PADDR,
    input           PWRITE,
    input   [31:0]  PWDATA,
    output  [31:0]  PRDATA,
    output          PREADY
);

    // Internal register declarations
    reg [31:0] CTRL_REG;

    // Write Logic
    always @(posedge PCLK or negedge PRESETn)
    begin
        if (!PRESETn)
        begin
            CTRL_REG <= 32'h1;
        end
        else if (PSEL && PENABLE && PWRITE)
        begin
            case (PADDR)
                32'h4: begin
                    CTRL_REG[0:0] <= PWDATA[0:0];
                    CTRL_REG[1:1] <= PWDATA[1:1];
                end
            endcase
        end
    end

    // Read Logic
    reg [31:0] prdata_reg;
    always @(*)
    begin
        case (PADDR)
            32'h4: prdata_reg = CTRL_REG;
            default: prdata_reg = 32'h0;
        endcase
    end

    assign PRDATA = prdata_reg;
    assign PREADY = 1'b1;

endmodule
