﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADMitroSremEmploye.Data.Migrations
{
    /// <inheritdoc />
    public partial class FMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankAccount = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dokument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatumDokumenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NazivDokumenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojDokumenta = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dokument", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeSalary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalNumberOfHours = table.Column<int>(type: "int", nullable: false),
                    TotalNumberOfWorkingHours = table.Column<int>(type: "int", nullable: false),
                    Sickness100 = table.Column<int>(type: "int", nullable: false),
                    Sickness60 = table.Column<int>(type: "int", nullable: false),
                    HoursOfAnnualVacation = table.Column<int>(type: "int", nullable: false),
                    WorkingHoursForTheHoliday = table.Column<int>(type: "int", nullable: false),
                    OvertimeHours = table.Column<int>(type: "int", nullable: false),
                    Credits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DamageCompensation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HolidayBonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MealAllowance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SettlementDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CalculationMonth = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeSalary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Komintenti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Komintent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Komintenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proizvod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SifraProizvoda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NazivProizvoda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoreskaGrupa = table.Column<int>(type: "int", nullable: false),
                    CenaProizvoda = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CenaProizvodaBezPdv = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ZaliheProizvoda = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    NazivProizvodaZaPrikaz = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proizvod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StateObligation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PIO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HealthCare = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    Unemployment = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    TaxRelief = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateObligation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameOfParent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    JMBG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IdentityCardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfEmployment = table.Column<DateOnly>(type: "date", nullable: false),
                    PIO = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    School = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    College = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmploymentContract = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AmendmentContract = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeBankAccount = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employe_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Otpremnica",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Paritet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojFiskalnogRacuna = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DokumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Otpremnica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Otpremnica_Dokument_DokumentId",
                        column: x => x.DokumentId,
                        principalTable: "Dokument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeSalarySO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionPension = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionHealth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseOfTheEmployer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeSalaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeSalarySO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeSalarySO_EmployeSalary_EmployeSalaryId",
                        column: x => x.EmployeSalaryId,
                        principalTable: "EmployeSalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeSalarySOE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionPension = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionHealth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionUnemployment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionTaxRelief = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseOfTheEmploye = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetoSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeSalaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeSalarySOE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeSalarySOE_EmployeSalary_EmployeSalaryId",
                        column: x => x.EmployeSalaryId,
                        principalTable: "EmployeSalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeFromWork",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkinHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Sickness60 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Sickness100 = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AnnualVacation = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HolidayHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Demage = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HotMeal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Regres = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinuliRad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EmployeSalaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeFromWork", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeFromWork_EmployeSalary_EmployeSalaryId",
                        column: x => x.EmployeSalaryId,
                        principalTable: "EmployeSalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kalkulacija",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DokumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KomintentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kalkulacija", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kalkulacija_Dokument_DokumentId",
                        column: x => x.DokumentId,
                        principalTable: "Dokument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kalkulacija_Komintenti_KomintentId",
                        column: x => x.KomintentId,
                        principalTable: "Komintenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Povratnica",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DokumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KomintentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Povratnica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Povratnica_Dokument_DokumentId",
                        column: x => x.DokumentId,
                        principalTable: "Dokument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Povratnica_Komintenti_KomintentId",
                        column: x => x.KomintentId,
                        principalTable: "Komintenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prijemnica",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DokumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KomintentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prijemnica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prijemnica_Dokument_DokumentId",
                        column: x => x.DokumentId,
                        principalTable: "Dokument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prijemnica_Komintenti_KomintentId",
                        column: x => x.KomintentId,
                        principalTable: "Komintenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Racun",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PIB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaticniBroj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Primalac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KomintentiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Paritet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojFiskalnogRacuna = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DokumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Racun_Dokument_DokumentId",
                        column: x => x.DokumentId,
                        principalTable: "Dokument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Racun_Komintenti_KomintentiId",
                        column: x => x.KomintentiId,
                        principalTable: "Komintenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnualLeaves",
                columns: table => new
                {
                    AnnualLeaveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ApprovalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    UsedDays = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    IsCarryOver = table.Column<bool>(type: "bit", nullable: false),
                    IsSickLeave = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualLeaves", x => x.AnnualLeaveId);
                    table.ForeignKey(
                        name: "FK_AnnualLeaves_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnnualLeaves_Employe_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeChild",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    JMBG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    EmployeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeChild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeChild_Employe_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OtpremnicaStavke",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IzlaznaKolicina = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IzlaznaVrednost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PDV = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CenaBezPdv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdvUDin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProizvodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OtpremnicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpremnicaStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpremnicaStavke_Otpremnica_OtpremnicaId",
                        column: x => x.OtpremnicaId,
                        principalTable: "Otpremnica",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OtpremnicaStavke_Proizvod_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KalkulacijaStavke",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UlaznaKolicina = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UlaznaVrednost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProizvodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kolicina = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UlaznaCena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PDV = table.Column<int>(type: "int", nullable: false),
                    NabavnaCena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NabavnaVrednost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VrednostRobeBezPdv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdvUDin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VrednostRobeSaPdv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CenaProizvodaBezPdv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CenaProizvodaSaPdv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KalkulacijaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KalkulacijaStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KalkulacijaStavke_Kalkulacija_KalkulacijaId",
                        column: x => x.KalkulacijaId,
                        principalTable: "Kalkulacija",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KalkulacijaStavke_Proizvod_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PovratnicaStavke",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UlaznaKolicina = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UlaznaVrednost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProizvodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PovratnicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PovratnicaStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PovratnicaStavke_Povratnica_PovratnicaId",
                        column: x => x.PovratnicaId,
                        principalTable: "Povratnica",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PovratnicaStavke_Proizvod_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrijemnicaStavke",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UlaznaKolicina = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UlaznaVrednost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProizvodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrijemnicaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrijemnicaStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrijemnicaStavke_Prijemnica_PrijemnicaId",
                        column: x => x.PrijemnicaId,
                        principalTable: "Prijemnica",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrijemnicaStavke_Proizvod_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RacunStavke",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IzlaznaKolicina = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    IzlaznaVrednost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PDV = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CenaBezPdv = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PdvUDin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProizvodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RacunId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacunStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RacunStavke_Proizvod_ProizvodId",
                        column: x => x.ProizvodId,
                        principalTable: "Proizvod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RacunStavke_Racun_RacunId",
                        column: x => x.RacunId,
                        principalTable: "Racun",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "StateObligation",
                columns: new[] { "Id", "Discriminator", "HealthCare", "PIO", "Tax", "TaxRelief", "Unemployment" },
                values: new object[] { new Guid("3be3823d-b2df-413b-a9c3-70f2379ca720"), "StateObligationsEmploye", 0.0515m, 0.14m, 0.10m, 25000m, 0.0075m });

            migrationBuilder.InsertData(
                table: "StateObligation",
                columns: new[] { "Id", "Discriminator", "HealthCare", "PIO" },
                values: new object[] { new Guid("9f2597c4-e985-4b36-b3f5-0ba1875e3ee9"), "StateObligation", 0.0515m, 0.10m });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualLeaves_CreatedByUserId",
                table: "AnnualLeaves",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualLeaves_EmployeId",
                table: "AnnualLeaves",
                column: "EmployeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employe_BankId",
                table: "Employe",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeChild_EmployeId",
                table: "EmployeChild",
                column: "EmployeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeSalarySO_EmployeSalaryId",
                table: "EmployeSalarySO",
                column: "EmployeSalaryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeSalarySOE_EmployeSalaryId",
                table: "EmployeSalarySOE",
                column: "EmployeSalaryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeFromWork_EmployeSalaryId",
                table: "IncomeFromWork",
                column: "EmployeSalaryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kalkulacija_DokumentId",
                table: "Kalkulacija",
                column: "DokumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Kalkulacija_KomintentId",
                table: "Kalkulacija",
                column: "KomintentId");

            migrationBuilder.CreateIndex(
                name: "IX_KalkulacijaStavke_KalkulacijaId",
                table: "KalkulacijaStavke",
                column: "KalkulacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_KalkulacijaStavke_ProizvodId",
                table: "KalkulacijaStavke",
                column: "ProizvodId");

            migrationBuilder.CreateIndex(
                name: "IX_Otpremnica_DokumentId",
                table: "Otpremnica",
                column: "DokumentId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpremnicaStavke_OtpremnicaId",
                table: "OtpremnicaStavke",
                column: "OtpremnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpremnicaStavke_ProizvodId",
                table: "OtpremnicaStavke",
                column: "ProizvodId");

            migrationBuilder.CreateIndex(
                name: "IX_Povratnica_DokumentId",
                table: "Povratnica",
                column: "DokumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Povratnica_KomintentId",
                table: "Povratnica",
                column: "KomintentId");

            migrationBuilder.CreateIndex(
                name: "IX_PovratnicaStavke_PovratnicaId",
                table: "PovratnicaStavke",
                column: "PovratnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_PovratnicaStavke_ProizvodId",
                table: "PovratnicaStavke",
                column: "ProizvodId");

            migrationBuilder.CreateIndex(
                name: "IX_Prijemnica_DokumentId",
                table: "Prijemnica",
                column: "DokumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Prijemnica_KomintentId",
                table: "Prijemnica",
                column: "KomintentId");

            migrationBuilder.CreateIndex(
                name: "IX_PrijemnicaStavke_PrijemnicaId",
                table: "PrijemnicaStavke",
                column: "PrijemnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_PrijemnicaStavke_ProizvodId",
                table: "PrijemnicaStavke",
                column: "ProizvodId");

            migrationBuilder.CreateIndex(
                name: "IX_Racun_DokumentId",
                table: "Racun",
                column: "DokumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Racun_KomintentiId",
                table: "Racun",
                column: "KomintentiId");

            migrationBuilder.CreateIndex(
                name: "IX_RacunStavke_ProizvodId",
                table: "RacunStavke",
                column: "ProizvodId");

            migrationBuilder.CreateIndex(
                name: "IX_RacunStavke_RacunId",
                table: "RacunStavke",
                column: "RacunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualLeaves");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "EmployeChild");

            migrationBuilder.DropTable(
                name: "EmployeSalarySO");

            migrationBuilder.DropTable(
                name: "EmployeSalarySOE");

            migrationBuilder.DropTable(
                name: "IncomeFromWork");

            migrationBuilder.DropTable(
                name: "KalkulacijaStavke");

            migrationBuilder.DropTable(
                name: "OtpremnicaStavke");

            migrationBuilder.DropTable(
                name: "PovratnicaStavke");

            migrationBuilder.DropTable(
                name: "PrijemnicaStavke");

            migrationBuilder.DropTable(
                name: "RacunStavke");

            migrationBuilder.DropTable(
                name: "StateObligation");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Employe");

            migrationBuilder.DropTable(
                name: "EmployeSalary");

            migrationBuilder.DropTable(
                name: "Kalkulacija");

            migrationBuilder.DropTable(
                name: "Otpremnica");

            migrationBuilder.DropTable(
                name: "Povratnica");

            migrationBuilder.DropTable(
                name: "Prijemnica");

            migrationBuilder.DropTable(
                name: "Proizvod");

            migrationBuilder.DropTable(
                name: "Racun");

            migrationBuilder.DropTable(
                name: "Bank");

            migrationBuilder.DropTable(
                name: "Dokument");

            migrationBuilder.DropTable(
                name: "Komintenti");
        }
    }
}
