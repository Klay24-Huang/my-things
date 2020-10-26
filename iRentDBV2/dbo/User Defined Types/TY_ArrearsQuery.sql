CREATE TYPE [dbo].[TY_ArrearsQuery] AS TABLE (
    [CUSTID]       VARCHAR (10)   NULL,
    [ORDNO]        VARCHAR (20)   NULL,
    [CNTRNO]       VARCHAR (20)   NULL,
    [PAYMENTTYPE]  VARCHAR (5)    NULL,
    [SPAYMENTTYPE] NVARCHAR (100) NULL,
    [DUEAMT]       INT            NULL,
    [PAIDAMT]      INT            NULL,
    [CARNO]        VARCHAR (30)   NULL,
    [POLNO]        VARCHAR (50)   NULL,
    [PAYTYPE]      VARCHAR (5)    NULL,
    [GIVEDATE]     VARCHAR (16)   NULL,
    [RNTDATE]      VARCHAR (16)   NULL,
    [INBRNHCD]     VARCHAR (5)    NULL,
    [IRENTORDNO]   VARCHAR (10)   NULL,
    [TAMT]         INT            NULL);

