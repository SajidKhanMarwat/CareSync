using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareSync.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_LabReports",
                columns: table => new
                {
                    LabReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabRequestID = table.Column<int>(type: "int", nullable: false),
                    PatientID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByDoctorID = table.Column<int>(type: "int", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LabReports", x => x.LabReportID);
                });

            migrationBuilder.CreateTable(
                name: "T_Labs",
                columns: table => new
                {
                    LabID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LabName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Labs", x => x.LabID);
                });

            migrationBuilder.CreateTable(
                name: "T_PatientReports",
                columns: table => new
                {
                    PatientReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    DocterID = table.Column<int>(type: "int", nullable: true),
                    PatientID = table.Column<int>(type: "int", nullable: true),
                    Documnt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PatientReports", x => x.PatientReportID);
                });

            migrationBuilder.CreateTable(
                name: "T_Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "T_LabServices",
                columns: table => new
                {
                    LabServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabID = table.Column<int>(type: "int", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SampleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T_LabLabID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LabServices", x => x.LabServiceID);
                    table.ForeignKey(
                        name: "FK_T_LabServices_T_Labs_T_LabLabID",
                        column: x => x.T_LabLabID,
                        principalTable: "T_Labs",
                        principalColumn: "LabID");
                });

            migrationBuilder.CreateTable(
                name: "T_RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", maxLength: 128, nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_RoleClaims_T_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "T_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoleID = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    LoginID = table.Column<int>(type: "int", nullable: false),
                    ArabicUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleType = table.Column<int>(type: "int", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_T_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_Users_T_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "T_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_T_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "T_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_DoctorDetails",
                columns: table => new
                {
                    DoctorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExperienceYears = table.Column<int>(type: "int", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QualificationSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HospitalAffiliation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsultationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AvailableDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DoctorDetails", x => x.DoctorID);
                    table.ForeignKey(
                        name: "FK_T_DoctorDetails_T_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "T_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PatientDetails",
                columns: table => new
                {
                    PatientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelationshipToEmergency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PatientDetails", x => x.PatientID);
                    table.ForeignKey(
                        name: "FK_T_PatientDetails_T_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "T_Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "T_UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_T_UserLogins_T_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "T_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_T_UserRoles_T_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "T_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_UserRoles_T_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "T_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_T_UserTokens_T_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "T_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_Qualifications",
                columns: table => new
                {
                    QualificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorID = table.Column<int>(type: "int", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfCompletion = table.Column<int>(type: "int", nullable: false),
                    Certificate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T_DoctorDetailsDoctorID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Qualifications", x => x.QualificationID);
                    table.ForeignKey(
                        name: "FK_T_Qualifications_T_DoctorDetails_T_DoctorDetailsDoctorID",
                        column: x => x.T_DoctorDetailsDoctorID,
                        principalTable: "T_DoctorDetails",
                        principalColumn: "DoctorID");
                });

            migrationBuilder.CreateTable(
                name: "T_AdditionalNotes",
                columns: table => new
                {
                    NoteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    NutritionalPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoctorRecommendations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_AdditionalNotes", x => x.NoteID);
                    table.ForeignKey(
                        name: "FK_T_AdditionalNotes_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_Appointments",
                columns: table => new
                {
                    AppointmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorID = table.Column<int>(type: "int", nullable: false),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Appointments", x => x.AppointmentID);
                    table.ForeignKey(
                        name: "FK_T_Appointments_T_DoctorDetails_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "T_DoctorDetails",
                        principalColumn: "DoctorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_T_Appointments_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_ChronicDiseases",
                columns: table => new
                {
                    ChronicDiseaseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    DiseaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagnosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ChronicDiseases", x => x.ChronicDiseaseID);
                    table.ForeignKey(
                        name: "FK_T_ChronicDiseases_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_LifestyleInfo",
                columns: table => new
                {
                    LifestyleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    IsSmoking = table.Column<bool>(type: "bit", nullable: true),
                    ExerciseFrequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExerciseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DailyActivity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOnDiet = table.Column<bool>(type: "bit", nullable: true),
                    DietType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SleepHours = table.Column<int>(type: "int", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LifestyleInfo", x => x.LifestyleID);
                    table.ForeignKey(
                        name: "FK_T_LifestyleInfo_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_MedicalFollowUps",
                columns: table => new
                {
                    FollowUpID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    LastVisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecentExaminations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpcomingReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DailyNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_MedicalFollowUps", x => x.FollowUpID);
                    table.ForeignKey(
                        name: "FK_T_MedicalFollowUps_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_MedicalHistories",
                columns: table => new
                {
                    MedicalHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    MainDiagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChronicDiseases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PastDiseases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surgery = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FamilyHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_MedicalHistories", x => x.MedicalHistoryID);
                    table.ForeignKey(
                        name: "FK_T_MedicalHistories_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_MedicationPlans",
                columns: table => new
                {
                    MedicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_MedicationPlans", x => x.MedicationID);
                    table.ForeignKey(
                        name: "FK_T_MedicationPlans_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_PatientVitals",
                columns: table => new
                {
                    VitalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PulseRate = table.Column<int>(type: "int", nullable: true),
                    BloodPressure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDiabetic = table.Column<bool>(type: "bit", nullable: true),
                    DiabeticReadings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasHighBloodPressure = table.Column<bool>(type: "bit", nullable: true),
                    BloodPressureReadings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PatientVitals", x => x.VitalID);
                    table.ForeignKey(
                        name: "FK_T_PatientVitals_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_LabRequests",
                columns: table => new
                {
                    LabRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    LabServiceID = table.Column<int>(type: "int", nullable: false),
                    RequestedByDoctorID = table.Column<int>(type: "int", nullable: true),
                    RequestedByPatientID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    T_AppointmentsAppointmentID = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_LabRequests", x => x.LabRequestID);
                    table.ForeignKey(
                        name: "FK_T_LabRequests_T_Appointments_T_AppointmentsAppointmentID",
                        column: x => x.T_AppointmentsAppointmentID,
                        principalTable: "T_Appointments",
                        principalColumn: "AppointmentID");
                });

            migrationBuilder.CreateTable(
                name: "T_Prescriptions",
                columns: table => new
                {
                    PrescriptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    DoctorID = table.Column<int>(type: "int", nullable: false),
                    PatientID = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Prescriptions", x => x.PrescriptionID);
                    table.ForeignKey(
                        name: "FK_T_Prescriptions_T_Appointments_AppointmentID",
                        column: x => x.AppointmentID,
                        principalTable: "T_Appointments",
                        principalColumn: "AppointmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Prescriptions_T_DoctorDetails_DoctorID",
                        column: x => x.DoctorID,
                        principalTable: "T_DoctorDetails",
                        principalColumn: "DoctorID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_Prescriptions_T_PatientDetails_PatientID",
                        column: x => x.PatientID,
                        principalTable: "T_PatientDetails",
                        principalColumn: "PatientID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "T_PrescriptionItems",
                columns: table => new
                {
                    PrescriptionItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionID = table.Column<int>(type: "int", nullable: false),
                    MedicineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_PrescriptionItems", x => x.PrescriptionItemID);
                    table.ForeignKey(
                        name: "FK_T_PrescriptionItems_T_Prescriptions_PrescriptionID",
                        column: x => x.PrescriptionID,
                        principalTable: "T_Prescriptions",
                        principalColumn: "PrescriptionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_AdditionalNotes_PatientID",
                table: "T_AdditionalNotes",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_Appointments_DoctorID",
                table: "T_Appointments",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_T_Appointments_PatientID",
                table: "T_Appointments",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_ChronicDiseases_PatientID",
                table: "T_ChronicDiseases",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_DoctorDetails_UserID",
                table: "T_DoctorDetails",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_T_LabRequests_T_AppointmentsAppointmentID",
                table: "T_LabRequests",
                column: "T_AppointmentsAppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_T_LabServices_T_LabLabID",
                table: "T_LabServices",
                column: "T_LabLabID");

            migrationBuilder.CreateIndex(
                name: "IX_T_LifestyleInfo_PatientID",
                table: "T_LifestyleInfo",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_MedicalFollowUps_PatientID",
                table: "T_MedicalFollowUps",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_MedicalHistories_PatientID",
                table: "T_MedicalHistories",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_MedicationPlans_PatientID",
                table: "T_MedicationPlans",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_PatientDetails_UserID",
                table: "T_PatientDetails",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_T_PatientVitals_PatientID",
                table: "T_PatientVitals",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_PrescriptionItems_PrescriptionID",
                table: "T_PrescriptionItems",
                column: "PrescriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_T_Prescriptions_AppointmentID",
                table: "T_Prescriptions",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_T_Prescriptions_DoctorID",
                table: "T_Prescriptions",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_T_Prescriptions_PatientID",
                table: "T_Prescriptions",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_T_Qualifications_T_DoctorDetailsDoctorID",
                table: "T_Qualifications",
                column: "T_DoctorDetailsDoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_T_RoleClaims_RoleId",
                table: "T_RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "T_Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_T_UserLogins_UserId",
                table: "T_UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_T_UserRoles_RoleId",
                table: "T_UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "T_Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_T_Users_RoleID",
                table: "T_Users",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "T_Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "T_AdditionalNotes");

            migrationBuilder.DropTable(
                name: "T_ChronicDiseases");

            migrationBuilder.DropTable(
                name: "T_LabReports");

            migrationBuilder.DropTable(
                name: "T_LabRequests");

            migrationBuilder.DropTable(
                name: "T_LabServices");

            migrationBuilder.DropTable(
                name: "T_LifestyleInfo");

            migrationBuilder.DropTable(
                name: "T_MedicalFollowUps");

            migrationBuilder.DropTable(
                name: "T_MedicalHistories");

            migrationBuilder.DropTable(
                name: "T_MedicationPlans");

            migrationBuilder.DropTable(
                name: "T_PatientReports");

            migrationBuilder.DropTable(
                name: "T_PatientVitals");

            migrationBuilder.DropTable(
                name: "T_PrescriptionItems");

            migrationBuilder.DropTable(
                name: "T_Qualifications");

            migrationBuilder.DropTable(
                name: "T_RoleClaims");

            migrationBuilder.DropTable(
                name: "T_UserLogins");

            migrationBuilder.DropTable(
                name: "T_UserRoles");

            migrationBuilder.DropTable(
                name: "T_UserTokens");

            migrationBuilder.DropTable(
                name: "T_Labs");

            migrationBuilder.DropTable(
                name: "T_Prescriptions");

            migrationBuilder.DropTable(
                name: "T_Appointments");

            migrationBuilder.DropTable(
                name: "T_DoctorDetails");

            migrationBuilder.DropTable(
                name: "T_PatientDetails");

            migrationBuilder.DropTable(
                name: "T_Users");

            migrationBuilder.DropTable(
                name: "T_Roles");
        }
    }
}
