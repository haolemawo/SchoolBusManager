using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WBPlatform.WebManagement.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    PropContent = table.Column<string>(nullable: true),
                    PropName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.ObjectId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    RealName = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    IsBusManager = table.Column<bool>(nullable: false),
                    IsClassTeacher = table.Column<bool>(nullable: false),
                    IsParent = table.Column<bool>(nullable: false),
                    AvatarPath = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Childs = table.Column<string>(nullable: true),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Precision = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ObjectId);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatorID = table.Column<string>(nullable: true),
                    SolverID = table.Column<string>(nullable: true),
                    DetailTexts = table.Column<string>(nullable: true),
                    RequestTypes = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ProcessResultReason = table.Column<int>(nullable: false),
                    NewContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_CreatorID",
                        column: x => x.CreatorID,
                        principalTable: "Users",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_SolverID",
                        column: x => x.SolverID,
                        principalTable: "Users",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CDepartment = table.Column<string>(nullable: true),
                    CGrade = table.Column<string>(nullable: true),
                    CNumber = table.Column<string>(nullable: true),
                    TeacherID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_Classes_Users_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "Users",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    SenderID = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Receivers = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Users",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SchoolBuses",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    BusName = table.Column<string>(nullable: true),
                    TeacherID = table.Column<string>(nullable: true),
                    BigWeekOnly = table.Column<bool>(nullable: false),
                    AHChecked = table.Column<bool>(nullable: false),
                    CSChecked = table.Column<bool>(nullable: false),
                    LSChecked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolBuses", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_SchoolBuses_Users_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "Users",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusReports",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    TeacherID = table.Column<string>(nullable: true),
                    BusID = table.Column<string>(nullable: true),
                    ReportType = table.Column<int>(nullable: false),
                    OtherData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusReports", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_BusReports_SchoolBuses_BusID",
                        column: x => x.BusID,
                        principalTable: "SchoolBuses",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BusReports_Users_TeacherID",
                        column: x => x.TeacherID,
                        principalTable: "Users",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    ObjectId = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    StudentName = table.Column<string>(nullable: true),
                    BusID = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(nullable: true),
                    ClassID = table.Column<string>(nullable: true),
                    LSChecked = table.Column<bool>(nullable: false),
                    CSChecked = table.Column<bool>(nullable: false),
                    AHChecked = table.Column<bool>(nullable: false),
                    TakingBus = table.Column<bool>(nullable: false),
                    WeekType = table.Column<int>(nullable: false),
                    DirectGoHome = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.ObjectId);
                    table.ForeignKey(
                        name: "FK_Students_SchoolBuses_BusID",
                        column: x => x.BusID,
                        principalTable: "SchoolBuses",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Classes_ClassID",
                        column: x => x.ClassID,
                        principalTable: "Classes",
                        principalColumn: "ObjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusReports_BusID",
                table: "BusReports",
                column: "BusID");

            migrationBuilder.CreateIndex(
                name: "IX_BusReports_ObjectId",
                table: "BusReports",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BusReports_TeacherID",
                table: "BusReports",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_CreatorID",
                table: "ChangeRequests",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ObjectId",
                table: "ChangeRequests",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_SolverID",
                table: "ChangeRequests",
                column: "SolverID");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ObjectId",
                table: "Classes",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherID",
                table: "Classes",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_Configs_ObjectId",
                table: "Configs",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ObjectId",
                table: "Notifications",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderID",
                table: "Notifications",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolBuses_ObjectId",
                table: "SchoolBuses",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolBuses_TeacherID",
                table: "SchoolBuses",
                column: "TeacherID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_BusID",
                table: "Students",
                column: "BusID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassID",
                table: "Students",
                column: "ClassID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ObjectId",
                table: "Students",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ObjectId",
                table: "Users",
                column: "ObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusReports");

            migrationBuilder.DropTable(
                name: "ChangeRequests");

            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "SchoolBuses");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
