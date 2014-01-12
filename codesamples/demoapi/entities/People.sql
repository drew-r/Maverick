--Copyright © 2013 Drew Rathbone.
--drewrathbone@gmail.com 
--
--This file is part of Maverick.
--
--Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
--as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
--You should have received a copy of the the GNU Affero General Public License, along with Maverick. 
--
--Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
--of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
--
--Additional permission under the GNU Affero GPL version 3 section 7: 
--If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
--that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
--
/****** Object:  Table [dbo].[People]    Script Date: 08/11/2013 21:00:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[People](
	[ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Forename] [nvarchar](50) NULL,
	[Surname] [nvarchar](50) NULL,
	[AuthUser] [nvarchar](50) NOT NULL,
	[AuthPass] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_People_AuthUser] UNIQUE NONCLUSTERED 
(
	[AuthUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[People] ADD  CONSTRAINT [DF_People_ID]  DEFAULT (newid()) FOR [ID]
GO

