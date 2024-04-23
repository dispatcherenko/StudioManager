using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudioManager.Migrations
{
    /// <inheritdoc />
    public partial class Usergames1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:sex", "М,Ж")
                .Annotation("Npgsql:PostgresExtension:pg_catalog.adminpack", ",,");

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id_department = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    departmentname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'DepartmentName'::character varying")
                },
                constraints: table =>
                {
                    table.PrimaryKey("departments_pkey", x => x.id_department);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id_game = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gamename = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'GameName'::character varying"),
                    gamegenre = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'GameGenre'::character varying"),
                    gamereleasedate = table.Column<DateOnly>(type: "date", nullable: false),
                    gamedescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("games_pkey", x => x.id_game);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userlogin = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'Login'::character varying"),
                    userpassword = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    useremail = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'example@example.com'::character varying"),
                    userprofilepicture = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    id_employee = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employeefullname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'FullName'::character varying"),
                    employeephonenumber = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "'+1234567890'::character varying"),
                    employeeemail = table.Column<string>(type: "character varying", nullable: false, defaultValueSql: "'example@example.com'::character varying"),
                    employeeposition = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'Employee'::character varying"),
                    employeesex = table.Column<string>(type: "text", unicode: false, nullable: false),
                    id_department = table.Column<int>(type: "integer", nullable: true),
                    employeephoto = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("staff_pkey", x => x.id_employee);
                    table.ForeignKey(
                        name: "id_department_fk",
                        column: x => x.id_department,
                        principalTable: "departments",
                        principalColumn: "id_department");
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id_task = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    taskgroup = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'TaskGroup'::character varying"),
                    taskname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValueSql: "'TaskName'::character varying"),
                    taskstate = table.Column<string>(type: "text", nullable: false, defaultValueSql: "'Sent'::character varying"),
                    id_department = table.Column<int>(type: "integer", nullable: true),
                    id_game = table.Column<int>(type: "integer", nullable: true),
                    taskdeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tasks_pkey", x => x.id_task);
                    table.ForeignKey(
                        name: "id_department_fk",
                        column: x => x.id_department,
                        principalTable: "departments",
                        principalColumn: "id_department");
                    table.ForeignKey(
                        name: "id_game_fk",
                        column: x => x.id_game,
                        principalTable: "games",
                        principalColumn: "id_game");
                });

            migrationBuilder.CreateTable(
                name: "usergames",
                columns: table => new
                {
                    id_game = table.Column<int>(type: "integer", nullable: false),
                    id_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usergames_pkey", x => new { x.id_game, x.id_user });
                    table.ForeignKey(
                        name: "id_game_fk",
                        column: x => x.id_game,
                        principalTable: "games",
                        principalColumn: "id_game",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "id_user_fk",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_staff_id_department",
                table: "staff",
                column: "id_department");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_id_department",
                table: "tasks",
                column: "id_department");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_id_game",
                table: "tasks",
                column: "id_game");

            migrationBuilder.CreateIndex(
                name: "IX_usergames_id_user",
                table: "usergames",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "usergames");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
