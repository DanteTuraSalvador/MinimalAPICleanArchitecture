using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestNest.Admin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeRoles",
                columns: table => new
                {
                    EmployeeRoleId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    RoleName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRoles", x => x.EmployeeRoleId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Establishments",
                columns: table => new
                {
                    EstablishmentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EstablishmentName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    EstablishmentEmail = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    EstablishmentStatusId = table.Column<int>(type: "INT", nullable: false, defaultValueSql: "(-1)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishments", x => x.EstablishmentId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaPlatforms",
                columns: table => new
                {
                    SocialMediaId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    SocialMediaName = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    PlatformURL = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMediaPlatforms", x => x.SocialMediaId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EmployeeNumber = table.Column<string>(type: "VARCHAR(10)", maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    EmailAddress = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    EmployeeStatusId = table.Column<int>(type: "INT", nullable: false, defaultValueSql: "(-1)"),
                    EmployeeRoleId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeeRoles_EmployeeRoleId",
                        column: x => x.EmployeeRoleId,
                        principalTable: "EmployeeRoles",
                        principalColumn: "EmployeeRoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "EstablishmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentAddresses",
                columns: table => new
                {
                    EstablishmentAddressId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    AddressLine = table.Column<string>(type: "NVARCHAR(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Municipality = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Province = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "FLOAT", nullable: false),
                    Longitude = table.Column<double>(type: "FLOAT", nullable: false),
                    IsPrimary = table.Column<bool>(type: "BIT", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentAddresses", x => x.EstablishmentAddressId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EstablishmentAddresses_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "EstablishmentId");
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentContacts",
                columns: table => new
                {
                    EstablishmentContactId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    ContactFirstName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    ContactMiddleName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    ContactLastName = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    ContactPhone = table.Column<string>(type: "VARCHAR(15)", maxLength: 15, nullable: false),
                    IsPrimary = table.Column<bool>(type: "BIT", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentContacts", x => x.EstablishmentContactId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EstablishmentContacts_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "EstablishmentId");
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentPhones",
                columns: table => new
                {
                    EstablishmentPhoneId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    PhoneNumber = table.Column<string>(type: "VARCHAR(15)", maxLength: 15, nullable: false),
                    IsPrimary = table.Column<bool>(type: "BIT", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentPhones", x => x.EstablishmentPhoneId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EstablishmentPhones_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "EstablishmentId");
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentSocialMedia",
                columns: table => new
                {
                    EstablishmentSocialMediaId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    SocialMediaId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    SocialMediaAccountName = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentSocialMedia", x => x.EstablishmentSocialMediaId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EstablishmentSocialMedia_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "EstablishmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentSocialMedia_SocialMediaPlatforms_SocialMediaId",
                        column: x => x.SocialMediaId,
                        principalTable: "SocialMediaPlatforms",
                        principalColumn: "SocialMediaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentMembers",
                columns: table => new
                {
                    EstablishmentMemberId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EstablishmentId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "UNIQUEIDENTIFIER", nullable: false),
                    MemberTitle = table.Column<string>(type: "NVARCHAR(100)", maxLength: 100, nullable: false),
                    MemberDescription = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false),
                    MemberTag = table.Column<string>(type: "NVARCHAR(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentMembers", x => x.EstablishmentMemberId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EstablishmentMembers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId");
                    table.ForeignKey(
                        name: "FK_EstablishmentMembers_Establishments_EstablishmentId",
                        column: x => x.EstablishmentId,
                        principalTable: "Establishments",
                        principalColumn: "EstablishmentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_RoleName",
                table: "EmployeeRoles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeRoleId",
                table: "Employees",
                column: "EmployeeRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EstablishmentId",
                table: "Employees",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentAddresses_EstablishmentId",
                table: "EstablishmentAddresses",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentContacts_EstablishmentId",
                table: "EstablishmentContacts",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentMembers_EmployeeId",
                table: "EstablishmentMembers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentMembers_EstablishmentId",
                table: "EstablishmentMembers",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentPhones_EstablishmentId",
                table: "EstablishmentPhones",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentSocialMedia_EstablishmentId",
                table: "EstablishmentSocialMedia",
                column: "EstablishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentSocialMedia_SocialMediaId",
                table: "EstablishmentSocialMedia",
                column: "SocialMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaName",
                table: "SocialMediaPlatforms",
                column: "SocialMediaName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstablishmentAddresses");

            migrationBuilder.DropTable(
                name: "EstablishmentContacts");

            migrationBuilder.DropTable(
                name: "EstablishmentMembers");

            migrationBuilder.DropTable(
                name: "EstablishmentPhones");

            migrationBuilder.DropTable(
                name: "EstablishmentSocialMedia");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "SocialMediaPlatforms");

            migrationBuilder.DropTable(
                name: "EmployeeRoles");

            migrationBuilder.DropTable(
                name: "Establishments");
        }
    }
}
