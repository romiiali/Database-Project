use PharmacyDatabase
--or 
-- create database PharmacyDatabase if u still didnt create it in ur sql app only run this once 

CREATE TABLE Supplier(
SupplierId INT NOT NULL PRIMARY KEY,
SupplierName VARCHAR(50),
Phone INT,
Email VARCHAR(50),
Address VARCHAR(50)
);

CREATE TABLE PharmacyStaff(
StaffId INT NOT NULL PRIMARY KEY,
Fname VARCHAR(50),
Lname VARCHAR(50),
Email VARCHAR(50),
Role VARCHAR(50)
);

CREATE TABLE PurchaseOrder(
PurchaseId INT NOT NULL PRIMARY KEY,
OrderDate DATE,
ExpectedDate DATE,
Status VARCHAR(50),
TotalAmount INT,
SupplierID INT,
StaffID INT
FOREIGN KEY(SupplierId) REFERENCES Supplier(SupplierId),
FOREIGN KEY(StaffID) REFERENCES PharmacyStaff(StaffId)
);

CREATE TABLE Category(
CategoryId INT NOT NULL PRIMARY Key,
CategoryName varchar(50),
Description varchar(50)
);


CREATE TABLE InventoryTransactions(
TransactionID INT NOT NULL PRIMARY Key,
TransactionDate date,
Quantity int,
RefrenceId int,
PurchaseId int, 
StaffId int,
FOREIGN KEY(PurchaseId) REFERENCES PurchaseOrder(PurchaseId),
FOREIGN KEY(StaffID) REFERENCES PharmacyStaff(StaffId)
);

CREATE TABLE InventoryTransactionType(
TransactionId int, 
TransactionType VARCHAR(50),
FOREIGN KEY(TransactionId) REFERENCES InventoryTransactions(TransactionID)
);

CREATE TABLE Medicine(
MedicineId INT NOT NULL PRIMARY Key,
ScientificName varchar(50),
CommercialName varchar(50),
Price int,
Dosage int,
ReorderQuantity varchar(50), 
CategoryId int,
FOREIGN KEY(CategoryId) REFERENCES Category(CategoryId)
);

CREATE TABLE Batch(
BatchId INT NOT NULL PRIMARY Key,
ExpiryDate date,
ArrivalDate date,
BatchNumber int,
QuantityRecieved int,
MedicineId int, 
SupplierId int,
FOREIGN KEY(MedicineId) REFERENCES Medicine(MedicineId),
FOREIGN KEY(SupplierId) REFERENCES Supplier(SupplierId));

CREATE TABLE StockAlert(
AlertId INT NOT NULL PRIMARY KEY,
AlertType VARCHAR(50),
AlertDateTime DATE,
Resolved BIT,
CurrentValueAlert INT,
ThresholdValue INT,
BatchId INT,
MedicineId INT,
TransactionId INT,
FOREIGN KEY(TransactionID) REFERENCES InventoryTransactions(TransactionID),
FOREIGN KEY(BatchId) REFERENCES Batch(BatchId),
FOREIGN KEY(MedicineId) REFERENCES Medicine(MedicineId)
);
CREATE TABLE Customer(
CustomerId INT NOT NULL PRIMARY Key,
Fname varchar(50),
Lname varchar(50),
Phone int,
Email varchar(50), 
);

CREATE TABLE SalesTransaction(
SaleId INT NOT NULL PRIMARY KEY,
SaleStatus VARCHAR(50),
SaleTime DATEtime,
Discount int,
TotalAmount INT,
CustomerId INT,
StaffId INT,
FOREIGN KEY(CustomerId) REFERENCES Customer(CustomerId),
FOREIGN KEY(StaffId) REFERENCES PharmacyStaff(StaffId)
);

CREATE TABLE SaleItem(
SaleItemId INT NOT NULL PRIMARY KEY,
Price int,
QuantitySold INT,
BatchId INT,
SaleId INT,
FOREIGN KEY(BatchId) REFERENCES Batch(BatchId),
FOREIGN KEY(SaleId) REFERENCES SalesTransaction(SaleId)
);

-- Delete all data in reverse dependency order
DELETE FROM StockAlert;
DELETE FROM SaleItem;
DELETE FROM SalesTransaction;
DELETE FROM InventoryTransactionType;
DELETE FROM InventoryTransactions;
DELETE FROM Batch;
DELETE FROM PurchaseOrder;
DELETE FROM Medicine;
DELETE FROM Customer;
DELETE FROM PharmacyStaff;
DELETE FROM Category;
DELETE FROM Supplier;

-- Use PharmacyDatabase (already done)

-- 1. Insert Categories (needed for Medicine)
INSERT INTO Category (CategoryId, CategoryName, Description) VALUES
(1, 'Pain Relief', 'Medications for pain management'),
(2, 'Antibiotics', 'Medications for bacterial infections'),
(3, 'Antihypertensives', 'Medications for high blood pressure'),
(4, 'Diabetes Care', 'Medications and supplies for diabetes'),
(5, 'Cold & Flu', 'Medications for cold symptoms'),
(6, 'Vitamins', 'Nutritional supplements and vitamins');

-- 2. Insert Suppliers (needed for PurchaseOrder, Batch)
INSERT INTO Supplier (SupplierId, SupplierName, Phone, Email, Address) VALUES
(1, 'PharmaCorp Ltd.', 5551001, 'john@pharmacorp.com', '123 Health St, Medcity'),
(2, 'Global Pharma Distributors', 5551002, 'maria@globalpharma.com', '456 Medical Blvd, Drugtown'),
(3, 'QuickMed Solutions', 5551003, 'robert@quickmed.com', '789 Wellness Ave, Medville'),
(4, 'PharmaPlus Distributors', 5551004, 'info@pharmaplus.com', '321 Cure Lane, Healthville'),
(5, 'MedExpress Supply Co.', 5551005, 'supply@medexpress.com', '654 Recovery Rd, Medtown');

-- 3. Insert Pharmacy Staff (needed for PurchaseOrder, SalesTransaction)
INSERT INTO PharmacyStaff (StaffId, Fname, Lname, Email, Role) VALUES
(1, 'John', 'Smith', 'john@pharmacy.com', 'Pharmacist'),
(2, 'Michael', 'Brown', 'michael@pharmacy.com', 'Pharmacy Technician'),
(3, 'Emily', 'Davis', 'emily@pharmacy.com', 'Cashier'),
(4, 'David', 'Wilson', 'david@pharmacy.com', 'Inventory Manager'),
(5, 'Jessica', 'Martinez', 'jessica@pharmacy.com', 'Pharmacist');

-- 4. Insert Medicines (needs Category, needed for Batch)
INSERT INTO Medicine (MedicineId, ScientificName, CommercialName, Price, Dosage, ReorderQuantity, CategoryId) VALUES
(1, 'Ibuprofen', 'Advil 200mg', 899, 200, '100 tablets', 1),
(2, 'Amoxicillin', 'Amoxicillin Capsules', 1550, 500, '50 capsules', 2),
(3, 'Lisinopril', 'Lisinopril Tablets', 1275, 10, '200 tablets', 3),
(4, 'Metformin', 'Metformin ER', 925, 500, '150 tablets', 4),
(5, 'Ascorbic Acid', 'Vitamin C 1000mg', 1499, 1000, '300 tablets', 6),
(6, 'Ibuprofen', 'Ibuprofen 200mg', 650, 200, '250 tablets', 1),
(7, 'Acetaminophen/Dextromethorphan', 'DayQuil Cold & Flu', 1099, 0, '100 bottles', 5),
(8, 'Atorvastatin', 'Atorvastatin 20mg', 1825, 20, '100 tablets', 3),
(9, 'Insulin Syringes', 'Insulin Syringes', 2599, 0, '500 units', 4),
(10, 'Cholecalciferol', 'Vitamin D3 5000IU', 1150, 5000, '200 capsules', 6);

-- 5. Insert Customers (needed for SalesTransaction)
INSERT INTO Customer (CustomerId, Fname, Lname, Phone, Email) VALUES
(1, 'Sarah', 'Johnson', 5553001, 'sarah.j@email.com'),
(2, 'Lisa', 'Anderson', 5553002, 'lisa.a@email.com'),
(3, 'Thomas', 'Miller', 5553003, 'thomas.m@email.com'),
(4, 'Jennifer', 'Lee', 5553004, 'jennifer.lee@email.com'),
(5, 'Robert', 'Taylor', 5553005, 'robert.t@email.com'),
(6, 'Maria', 'Garcia', 5553006, 'maria.g@email.com'),
(7, 'William', 'Clark', 5553007, 'william.c@email.com');

-- 6. Insert Purchase Orders (needs Supplier, PharmacyStaff, needed for InventoryTransactions)
INSERT INTO PurchaseOrder (PurchaseId, OrderDate, ExpectedDate, Status, TotalAmount, SupplierID, StaffID) VALUES
(1, '2024-04-01', '2024-04-05', 'Delivered', 125000, 1, 1),
(2, '2024-04-05', '2024-04-10', 'Processing', 162550, 2, 4),
(3, '2024-04-10', '2024-04-15', 'Delivered', 310075, 3, 4),
(4, '2024-04-12', '2024-04-17', 'Pending', 187500, 1, 4);

-- 7. Insert Batches (needs Medicine, Supplier, needed for SaleItem, StockAlert)
INSERT INTO Batch (BatchId, ExpiryDate, ArrivalDate, BatchNumber, QuantityRecieved, MedicineId, SupplierId) VALUES
(1, '2025-12-31', '2024-01-15', 202401, 500, 1, 1),
(2, '2026-02-10', '2024-02-15', 202402, 300, 2, 2),
(3, '2026-03-05', '2024-03-10', 202403, 400, 3, 1),
(4, '2025-07-20', '2024-01-25', 202404, 350, 4, 3),
(5, '2025-08-15', '2024-02-20', 202405, 600, 5, 2),
(6, '2026-04-01', '2024-04-05', 202406, 450, 6, 1),
(7, '2025-09-20', '2024-03-25', 202407, 250, 7, 3),
(8, '2026-02-28', '2024-03-03', 202408, 200, 8, 2),
(9, '2025-07-10', '2024-01-15', 202409, 1000, 9, 1),
(10, '2026-03-15', '2024-03-20', 202410, 550, 10, 3);

-- 8. Insert Inventory Transactions (needs PurchaseOrder, PharmacyStaff, needed for InventoryTransactionType, StockAlert)
INSERT INTO InventoryTransactions (TransactionID, TransactionDate, Quantity, RefrenceId, PurchaseId, StaffId) VALUES
(1, '2024-04-01', 500, 1001, 1, 1),
(2, '2024-04-02', 300, 1002, 1, 4),
(3, '2024-04-06', 400, 1003, 3, 4),
(4, '2024-04-06', 350, 1004, 3, 4),
(5, '2024-04-11', 600, 1005, 3, 4),
(6, '2024-04-11', 450, 1006, 3, 4),
(7, '2024-04-03', 250, 1007, 1, 4),
(8, '2024-04-03', 200, 1008, 1, 4);

-- 9. Insert Inventory Transaction Types (needs InventoryTransactions)
INSERT INTO InventoryTransactionType (TransactionId, TransactionType) VALUES
(1, 'Purchase'),
(2, 'Purchase'),
(3, 'Purchase'),
(4, 'Purchase'),
(5, 'Purchase'),
(6, 'Purchase'),
(7, 'Purchase'),
(8, 'Purchase');

-- 10. Insert Sales Transactions (needs Customer, PharmacyStaff, needed for SaleItem)
INSERT INTO SalesTransaction (SaleId, SaleStatus, SaleTime, Discount, TotalAmount, CustomerId, StaffId) VALUES
(1, 'Completed', '2024-04-11 10:15:00', 0, 2999, 1, 1),
(2, 'Completed', '2024-04-12 11:30:00', 500, 2099, 2, 3),
(3, 'Completed', '2024-04-13 14:45:00', 0, 6875, 3, 1),
(4, 'Completed', '2024-04-14 09:20:00', 0, 3324, 4, 3),
(5, 'Completed', '2024-04-15 16:10:00', 1000, 4050, 5, 1),
(6, 'Pending', '2024-04-15 17:30:00', 0, 6500, 6, 2);

-- 11. Insert Sale Items (needs Batch, SalesTransaction)
INSERT INTO SaleItem (SaleItemId, Price, QuantitySold, BatchId, SaleId) VALUES
(1, 899, 2, 1, 1),
(2, 1499, 1, 5, 1),
(3, 650, 3, 6, 1),
(4, 2599, 1, 9, 2),
(5, 1550, 2, 2, 3),
(6, 925, 1, 4, 3),
(7, 1150, 1, 10, 3),
(8, 1099, 2, 7, 4),
(9, 1499, 1, 5, 4),
(10, 1275, 3, 3, 5),
(11, 1825, 1, 8, 5),
(12, 650, 5, 6, 6),
(13, 899, 2, 1, 6);

-- 12. Insert Stock Alerts (needs Batch, Medicine, InventoryTransactions)
INSERT INTO StockAlert (AlertId, AlertType, AlertDateTime, Resolved, CurrentValueAlert, ThresholdValue, BatchId, MedicineId, TransactionId) VALUES
(1, 'Low Stock', '2024-03-20', 1, 15, 50, 1, 1, 1),
(2, 'Low Stock', '2024-04-12', 0, 199, 50, 8, 8, 8),
(3, 'Expiring Soon', '2024-04-01', 0, 998, 0, 9, 9, NULL),
(4, 'Reorder', '2024-04-10', 0, 45, 50, NULL, 6, NULL);