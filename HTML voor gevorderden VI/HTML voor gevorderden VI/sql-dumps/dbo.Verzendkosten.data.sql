SET IDENTITY_INSERT [dbo].[Verzendkosten] ON
INSERT INTO [dbo].[Verzendkosten] ([Id], [MinGewicht], [MaxGewicht], [Tarief]) VALUES (2, CAST(0.00 AS Decimal(5, 2)), CAST(0.50 AS Decimal(5, 2)), CAST(3.15 AS Decimal(5, 2)))
INSERT INTO [dbo].[Verzendkosten] ([Id], [MinGewicht], [MaxGewicht], [Tarief]) VALUES (3, CAST(0.50 AS Decimal(5, 2)), CAST(2.00 AS Decimal(5, 2)), CAST(3.70 AS Decimal(5, 2)))
INSERT INTO [dbo].[Verzendkosten] ([Id], [MinGewicht], [MaxGewicht], [Tarief]) VALUES (4, CAST(2.00 AS Decimal(5, 2)), CAST(10.00 AS Decimal(5, 2)), CAST(5.45 AS Decimal(5, 2)))
INSERT INTO [dbo].[Verzendkosten] ([Id], [MinGewicht], [MaxGewicht], [Tarief]) VALUES (5, CAST(10.00 AS Decimal(5, 2)), CAST(20.00 AS Decimal(5, 2)), CAST(9.95 AS Decimal(5, 2)))
INSERT INTO [dbo].[Verzendkosten] ([Id], [MinGewicht], [MaxGewicht], [Tarief]) VALUES (7, CAST(20.00 AS Decimal(5, 2)), CAST(990.00 AS Decimal(5, 2)), CAST(19.90 AS Decimal(5, 2)))
SET IDENTITY_INSERT [dbo].[Verzendkosten] OFF
