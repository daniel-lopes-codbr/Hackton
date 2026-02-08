-- InitPostgres.sql
-- SQL script to create initial schema for AgroSolutions on PostgreSQL
-- Run inside the Postgres container:
-- psql -U postgres -d db_ms_agro -f /path/to/InitPostgres.sql

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

BEGIN;

-- Users
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" varchar(200) NOT NULL,
    "Email" varchar(200) NOT NULL,
    "PasswordHash" varchar(500) NOT NULL,
    "Role" varchar(20) NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE INDEX IF NOT EXISTS "IX_Users_Role" ON "Users" ("Role");

-- Farms
CREATE TABLE IF NOT EXISTS "Farms" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" varchar(200) NOT NULL,
    "UserId" uuid NULL,
    "WidthMeters" numeric(18,2) NOT NULL,
    "LengthMeters" numeric(18,2) NOT NULL,
    "TotalAreaSquareMeters" numeric(18,2) NOT NULL,
    "Precipitation" numeric(18,2) NULL,
    "Property_Name" varchar(200) NULL,
    "Property_Location" varchar(500) NULL,
    "Property_Area" numeric(18,2) NULL,
    "Property_Description" varchar(1000) NULL,
    "OwnerName" text NULL,
    "OwnerEmail" text NULL,
    "OwnerPhone" text NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NULL,
    CONSTRAINT "FK_Farms_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS "IX_Farms_UserId" ON "Farms" ("UserId");

-- Fields
CREATE TABLE IF NOT EXISTS "Fields" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "FarmId" uuid NOT NULL,
    "Name" varchar(200) NOT NULL,
    "AreaSquareMeters" numeric(18,2) NOT NULL,
    "CropType" varchar(100) NOT NULL,
    "Property_Name" varchar(200) NULL,
    "Property_Location" varchar(500) NULL,
    "Property_Area" numeric(18,2) NULL,
    "Property_Description" varchar(1000) NULL,
    "PlantingDate" timestamptz NULL,
    "HarvestDate" timestamptz NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NULL,
    CONSTRAINT "FK_Fields_Farms_FarmId" FOREIGN KEY ("FarmId") REFERENCES "Farms"("Id") ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS "IX_Fields_FarmId" ON "Fields" ("FarmId");

-- SensorReadings
CREATE TABLE IF NOT EXISTS "SensorReadings" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "FieldId" uuid NOT NULL,
    "SensorType" varchar(50) NULL,
    "Value" numeric(18,4) NULL,
    "Unit" varchar(20) NULL,
    "ReadingTimestamp" timestamptz NULL,
    "Location" varchar(200) NULL,
    "Metadata" jsonb NULL,
    "SoilMoisture" numeric(18,4) NULL,
    "AirTemperature" numeric(18,4) NULL,
    "Precipitation" numeric(18,4) NULL,
    "IsRichInPests" boolean NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NULL,
    CONSTRAINT "FK_SensorReadings_Fields_FieldId" FOREIGN KEY ("FieldId") REFERENCES "Fields"("Id") ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS "IX_SensorReadings_FieldId" ON "SensorReadings" ("FieldId");
CREATE INDEX IF NOT EXISTS "IX_SensorReadings_ReadingTimestamp" ON "SensorReadings" ("ReadingTimestamp");
CREATE INDEX IF NOT EXISTS "IX_SensorReadings_CreatedAt" ON "SensorReadings" ("CreatedAt");

-- Alerts
CREATE TABLE IF NOT EXISTS "Alerts" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "FieldId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "IsEnable" boolean NOT NULL,
    "FarmId" uuid NULL,
    "Message" text NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NULL,
    CONSTRAINT "FK_Alerts_Fields_FieldId" FOREIGN KEY ("FieldId") REFERENCES "Fields"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Alerts_Farms_FarmId" FOREIGN KEY ("FarmId") REFERENCES "Farms"("Id") ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS "IX_Alerts_FieldId" ON "Alerts" ("FieldId");
CREATE INDEX IF NOT EXISTS "IX_Alerts_Status" ON "Alerts" ("Status");
CREATE INDEX IF NOT EXISTS "IX_Alerts_IsEnable" ON "Alerts" ("IsEnable");
CREATE INDEX IF NOT EXISTS "IX_Alerts_CreatedAt" ON "Alerts" ("CreatedAt");

-- UserAuthorizations
CREATE TABLE IF NOT EXISTS "UserAuthorizations" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" uuid NOT NULL,
    "PermissionType" text NOT NULL,
    "CreatedAt" timestamptz NOT NULL,
    "UpdatedAt" timestamptz NULL,
    CONSTRAINT "FK_UserAuthorizations_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

COMMIT;

