SET IDENTITY_INSERT [dbo].[Bezorgkosten] ON
INSERT INTO [dbo].[Bezorgkosten] ([Id], [Bezorgen], [Afhalen], [Avond], [Zakelijk]) VALUES (1, CAST(0.00 AS Decimal(10, 2)), CAST(0.00 AS Decimal(10, 2)), CAST(1.95 AS Decimal(10, 2)), CAST(2.95 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[Bezorgkosten] OFF
