CREATE DATABASE QuanLyQuanCafe
GO

USE QuanLyQuanCafe
GO

--Food
--Table
--FoodCategory
--Account
--Bill
--BillInfo

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Bàn chưa có tên',
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống', --Trống || Có người
)
GO

CREATE TABLE Account
(
	UserName NVARCHAR(100) PRIMARY KEY,
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'Kten',
	PassWord NVARCHAR(1000) NOT NULL  DEFAULT 0,
	TYPE INT NOT NULL  DEFAULT 0 --1:admin " 0:staff
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên'
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	idCategory INT NOT NULL,
	price FLOAT NOT NULL DEFAULT 0

	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0 -- 1:Đã thanh toán" 0: chưa thanh toán
	
	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0

	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO

INSERT INTO dbo.Account
(
	UserName,
	DisplayName,
	Password,
	Type
)
VALUES
(
	N'K9', -- UserNames-nvarchar(100)
	N'RongK9', -- DisplayName-nvarchar(100)
	N'1', -- PassWord-nvarchar(1000)
	1 -- Type -int
)
INSERT INTO dbo.Account
(
	UserName,
	DisplayName,
	Password,
	Type
)
VALUES
(
	N'Staff', -- UserNames-nvarchar(100)
	N'Staff', -- DisplayName-nvarchar(100)
	N'1', -- PassWord-nvarchar(1000)
	0 -- Type -int
)
GO

CREATE PROC USP_GetAccountByUserName
@userName nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE UserName=@userName
END
GO

CREATE PROC USP_Login
@userName nvarchar(100), @passWord nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.Account WHERE UserName = @userName AND PassWord = @passWord
END
GO
--thêm bảng
DECLARE @i INT=0

WHILE @i<=10
BEGIN
	INSERT dbo.TableFood(name) VALUES (N'Bàn ' +CAST(@i AS nvarchar(100)))
	SET @i=@i + 1
END
GO

CREATE PROC USP_GetTableList
AS SELECT * FROM dbo.TableFood 
GO

UPDATE dbo.TableFood SET status=N'Có người' WHERE id=2

EXEC dbo.USP_GetTableList
GO

--thêm category
INSERT dbo.FoodCategory(name)
VALUES (N'Hải sản') -- name - nvarchar(100)
INSERT dbo.FoodCategory(name)
VALUES (N'Nông sản')
INSERT dbo.FoodCategory(name)
VALUES (N'Lâm sản')
INSERT dbo.FoodCategory(name)
VALUES (N'Món khác')
INSERT dbo.FoodCategory(name)
VALUES (N'Nước')

--Thêm món ăn
INSERT dbo.Food(name,idCategory,price)
VALUES (N'Mực một nắng nướng sa tế',--name - nvarchar(100) 
		1, -- idCategory --int
		120000 )
INSERT dbo.Food(name,idCategory,price)
VALUES (N'Nghêu hấp xả',1,50000)
INSERT dbo.Food(name,idCategory,price)
VALUES (N'Dú dê nướng sữa',2,120000)
INSERT dbo.Food(name,idCategory,price)
VALUES (N'Heo rừng nướng muối',3,200000)
INSERT dbo.Food(name,idCategory,price)
VALUES (N'Cơm chiên su si',4,75000)
INSERT dbo.Food(name,idCategory,price)
VALUES (N'Chanh muối',5,5000)
INSERT dbo.Food(name,idCategory,price)
VALUES (N'Cafe',5,12000)

--Thêm bill
INSERT dbo.Bill(DateCheckIn,DateCheckOut,idTable,status)
VALUES (
	GETDATE(), 
	NULL,  
	3, 
	0 )
INSERT dbo.Bill(DateCheckIn,DateCheckOut,idTable,status)
VALUES (GETDATE(),NULL,4,0)
INSERT dbo.Bill(DateCheckIn,DateCheckOut,idTable,status)
VALUES (GETDATE(),GETDATE(),5,1)

--thêm bill Info
INSERT dbo.BillInfo(idBill,idFood,count)
VALUES (1,3,2)
INSERT dbo.BillInfo(idBill,idFood,count)
VALUES (2,2,4)
INSERT dbo.BillInfo(idBill,idFood,count)
VALUES (3,1,2)
INSERT dbo.BillInfo(idBill,idFood,count)
VALUES (2,2,3)
INSERT dbo.BillInfo(idBill,idFood,count)
VALUES (1,1,2)

GO

ALTER TABLE dbo.Bill
ADD discount INT

UPDATE dbo.Bill SET discount = 0
GO

CREATE PROC USP_InsertBill
@idTable INT
AS
BEGIN
	INSERT dbo.Bill(DateCheckIn,DateCheckOut,idTable,status,discount)
	VALUES (GETDATE(),NULL,@idTable,0,0)
END
GO


CREATE PROC USP_InsertBillInfo
@idBill INT,@idFood INT,@count INT
AS
BEGIN
	DECLARE @isExitsBillInfo INT
	DECLARE @foodCount INT =1

	SELECT @isExitsBillInfo = id,@foodCount=b.count 
	FROM dbo.BillInfo AS b 
	WHERE idBill=@idBill AND idFood=@idFood

	IF(@isExitsBillInfo > 0)
	BEGIN
		DECLARE	@newCount INT = @foodCount + @count 
		IF (@newCount> 0)
			UPDATE dbo.BillInfo SET count= @foodCount + @count WHERE idFood=@idFood
		ELSE
			DELETE dbo.BillInfo WHERE idBill=@idBill AND idFood=@idFood
	END
	ELSE
	BEGIN
		INSERT dbo.BillInfo(idBill,idFood,count)
		VALUES (@idBill,@idFood,@count)
	END
END
GO


CREATE TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT 

	SELECT @idBill = idBill FROM inserted

	DECLARE @idTable INT

	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status=0

	DECLARE @count INT
	SELECT @count = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idBill
	IF(@count > 0)
		UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable
	ELSE
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO

CREATE TRIGGER UTG_UpdateBill
ON dbo.Bill FOR UPDATE
AS
BEGIN
	DECLARE @idBill INT

	SELECT @idBill = id FROM inserted

	DECLARE @idTable INT

	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill 

	DECLARE @count int = 0

	SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable= @idTable AND status=0

	IF(@count=0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id=@idTable
END
GO


CREATE PROC USP_SwitchTabel
@idTale1 INT, @idTable2 int
AS
BEGIN
	DECLARE @idFirstBill int
	DECLARE @idSeconrdBill int

	DECLARE @isFirstTableEmty INT = 1
	DECLARE @isSecondTableEmty INT = 1

	SELECT @idSeconrdBill = id FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	SELECT @idFirstBill = id FROM dbo.Bill WHERE idTable = @idTale1 AND status = 0

	IF (@idFirstBill IS NULL)
	BEGIN
		INSERT dbo.Bill(DateCheckIn,DateCheckOut,idTable,status)
		VALUES( GETDATE(),NULL,@idTale1,0)
		SELECT @idFirstBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTale1 AND status = 0
		
	END

	SELECT @isFirstTableEmty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idFirstBill

	IF (@idSeconrdBill IS NULL)
	BEGIN
		INSERT dbo.Bill(DateCheckIn,DateCheckOut,idTable,status)
		VALUES (GETDATE(),NULL,@idTable2,0)
		SELECT @idSeconrdBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
		
	END

	SELECT @isSecondTableEmty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idSeconrdBill

	SELECT id INTO IDBillInfoTable FROM dbo.BillInfo WHERE idBill = @idSeconrdBill
	
	UPDATE dbo.BillInfo SET idBill = @idSeconrdBill WHERE idBill = @idFirstBill 
	
	UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id IN (SELECT * FROM IDBillInfoTable)

	DROP TABLE IDBillInfoTable

	IF(@isFirstTableEmty = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable2
	IF(@isSecondTableEmty = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTale1
END
GO


ALTER TABLE dbo.Bill ADD totalPrice FLOAT
GO

CREATE PROC USP_GetListBillByDate
@checkIn date, @checkOut date
AS
BEGIN
	SELECT t.name AS [Tên bàn],b.totalPrice AS [Thành tiền], DateCheckIn AS [Ngày vào] ,DateCheckOut AS [Ngày ra],discount AS [Giảm giá] 
	FROM dbo.Bill AS b,dbo.TableFood AS t
	WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1
	AND t.id = b.idTable
END
GO

CREATE PROC USP_UpdateAccount
@userName NVARCHAR(100), @displayName NVARCHAR(100), @password NVARCHAR(100), @newPassword NVARCHAR(100)
AS
BEGIN
	DECLARE @isRightPass INT = 0

	SELECT @isRightPass = COUNT(*) FROM dbo.Account WHERE UserName= @userName AND PassWord = @password

	IF(@isRightPass = 1)
	BEGIN
		IF(@newPassword = NULL OR @newPassword = '')
		BEGIN
			UPDATE dbo.Account SET DisplayName = @displayName WHERE UserName = @userName
		END
		ELSE
		UPDATE dbo.Account SET DisplayName = @displayName, PassWord = @newPassword WHERE UserName = @userName
	END

END
GO

CREATE TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS
BEGIN
	DECLARE @idBillInfo INT
	DECLARE @idBill INT 
	SELECT @idBillInfo = id, @idBill = deleted.idBill FROM deleted

	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill

	DECLARE @count INT = 0
	SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi, dbo.Bill AS b WHERE b.id = bi.idBill AND b.id= @idBill AND b.status = 0

	IF(@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO

CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END
GO

CREATE PROC USP_GetListBillByDateAndDate
@checkIn date, @checkOut date, @page int
AS
BEGIN
	DECLARE @pageRows INT = 10
	DECLARE @selectRows INT = @pageRows
	DECLARE @exceptRows INT = (@page - 1) * @pageRows

	;WITH BillShow AS (SELECT b.ID, t.name AS [Tên bàn],b.totalPrice AS [Thành tiền], DateCheckIn AS [Ngày vào] ,DateCheckOut AS [Ngày ra],discount AS [Giảm giá] 
	FROM dbo.Bill AS b,dbo.TableFood AS t
	WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1
	AND t.id = b.idTable)
	
	SELECT TOP (@selectRows) * FROM BillShow WHERE id NOT IN (SELECT TOP (@exceptRows) id FROM BillShow)
	
END
GO

CREATE PROC USP_GetNumBillListByDate
@checkIn date, @checkOut date
AS
BEGIN
	SELECT COUNT(*)
	FROM dbo.Bill AS b,dbo.TableFood AS t
	WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1
	AND t.id = b.idTable
END
GO

CREATE PROC USP_DeleteTableFoodByID
@id INT
AS
BEGIN
--xóa bảng BillInfo
	DECLARE @idBill INT 
	SELECT @idBill=id from dbo.Bill WHERE idTable = @id -- chọn ra những mã hóa đơn có mã bàn truyền vào
	DELETE dbo.BillInfo WHERE idBill = @idBill-- xóa bảng chi tiết hóa đơn với cái mã hóa đơn dc chọn từ hóa đơn
--xóa bảng Bill
	DELETE dbo.Bill WHERE idTable=@id-- xóa hóa đơn có cái mã bàn truyền vào
--xóa bảng TableFood
	DELETE dbo.TableFood WHERE id =@id-- xóa bàn
END
GO
--exec USP_DeleteTableFoodBID @id = 10

CREATE PROC USP_DeleteFoodCategoryByID
@id INT
AS
BEGIN
--xóa bảng BillInfo
	DECLARE @idFood INT 
	SELECT @idFood=id FROM dbo.Food WHERE idCategory = @id -- chọn ra những mã thức ăn có mã danh mục thức ăn truyền vào
	DELETE dbo.BillInfo WHERE idFood = @idFood-- xóa bảng chi tiết hóa đơn với cái mã thức ăn dc chọn từ bảng thức ăn
--xóa bảng Food
	DELETE dbo.Food WHERE idCategory=@id-- xóa bảng thức ăn có cái mã danh mục thức ăn truyền vào
--xóa bảng FoodCategory
	DELETE dbo.FoodCategory WHERE id =@id-- xóa danh mục thức ăn
END
GO
--exec USP_DeleteFoodCategoryByID @id = 1


EXEC dbo.USP_GetListBillByDateAndDate @checkIn = '2019-1-01', @checkOut = '2019-12-02', @page = 1

--UPDATE dbo.FoodCategory SET name = N'' WHERE id = 1
--UPDATE dbo.Account SET UserName = N'', DisplayName = N'',TYPE = 1 

--DELETE dbo.FoodCategory WHERE id =1
SELECT UserName,DisplayName,TYPE FROM dbo.Account

SELECT * FROM dbo.TableFood

SELECT * FROM dbo.Bill

SELECT * FROM dbo.BillInfo

SELECT id as [Mã thức ăn], name as [Tên thức ăn], idCategory as [Mã danh mục], price as [Giá tiền] FROM dbo.Food

SELECT * FROM dbo.FoodCategory

SELECT * FROM Account 

SELECT * FROM Account WHERE UserName = N'k9'

--DELETE dbo.BillInfo where idBill =2
--DELETE dbo.Bill where idTable = 1
--DELETE dbo.TableFood where id =1
--EXEC dbo.USP_SwitchTabel @idTale1 = 3,@idTable2 = 4

