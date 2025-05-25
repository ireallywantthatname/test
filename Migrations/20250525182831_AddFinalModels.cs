using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Quizzes",
                newName: "QuizName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Quizzes",
                newName: "QuizId");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "Questions",
                newName: "QuizID");

            migrationBuilder.RenameColumn(
                name: "Difficulty",
                table: "Questions",
                newName: "QuestionBody");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Questions",
                newName: "QuestionID");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                newName: "IX_Questions_QuizID");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "Answers",
                newName: "QuestionID");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Answers",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Answers",
                newName: "AnswerID");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionId",
                table: "Answers",
                newName: "IX_Answers_QuestionID");

            migrationBuilder.AddColumn<int>(
                name: "EducatorID",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "QuizDescription",
                table: "Quizzes",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QuizDifficulty",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionDifficulty",
                table: "Questions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttemptID",
                table: "Answers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionPartID",
                table: "Answers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "Answers",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "Educators",
                columns: table => new
                {
                    EducatorID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                    ConfirmPassword = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educators", x => x.EducatorID);
                });

            migrationBuilder.CreateTable(
                name: "QuestionParts",
                columns: table => new
                {
                    QuestionPartID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionID = table.Column<int>(type: "INTEGER", nullable: false),
                    Body = table.Column<string>(type: "TEXT", maxLength: 192, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionParts", x => x.QuestionPartID);
                    table.ForeignKey(
                        name: "FK_QuestionParts_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "QuestionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false),
                    ConfirmPassword = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    AttemptID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentID = table.Column<int>(type: "INTEGER", nullable: false),
                    QuizID = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalScore = table.Column<float>(type: "REAL", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.AttemptID);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentEducators",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "INTEGER", nullable: false),
                    EducatorID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEducators", x => new { x.StudentID, x.EducatorID });
                    table.ForeignKey(
                        name: "FK_StudentEducators_Educators_EducatorID",
                        column: x => x.EducatorID,
                        principalTable: "Educators",
                        principalColumn: "EducatorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentEducators_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_EducatorID",
                table: "Quizzes",
                column: "EducatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_AttemptID",
                table: "Answers",
                column: "AttemptID");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionPartID",
                table: "Answers",
                column: "QuestionPartID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionParts_QuestionID",
                table: "QuestionParts",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_QuizID",
                table: "QuizAttempts",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_StudentID",
                table: "QuizAttempts",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEducators_EducatorID",
                table: "StudentEducators",
                column: "EducatorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_QuestionParts_QuestionPartID",
                table: "Answers",
                column: "QuestionPartID",
                principalTable: "QuestionParts",
                principalColumn: "QuestionPartID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionID",
                table: "Answers",
                column: "QuestionID",
                principalTable: "Questions",
                principalColumn: "QuestionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_QuizAttempts_AttemptID",
                table: "Answers",
                column: "AttemptID",
                principalTable: "QuizAttempts",
                principalColumn: "AttemptID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_QuizID",
                table: "Questions",
                column: "QuizID",
                principalTable: "Quizzes",
                principalColumn: "QuizId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Educators_EducatorID",
                table: "Quizzes",
                column: "EducatorID",
                principalTable: "Educators",
                principalColumn: "EducatorID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_QuestionParts_QuestionPartID",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionID",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Answers_QuizAttempts_AttemptID",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_QuizID",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Educators_EducatorID",
                table: "Quizzes");

            migrationBuilder.DropTable(
                name: "QuestionParts");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "StudentEducators");

            migrationBuilder.DropTable(
                name: "Educators");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_EducatorID",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Answers_AttemptID",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionPartID",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "EducatorID",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuizDescription",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuizDifficulty",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuestionDifficulty",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AttemptID",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "QuestionPartID",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "QuizName",
                table: "Quizzes",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "QuizId",
                table: "Quizzes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "QuizID",
                table: "Questions",
                newName: "QuizId");

            migrationBuilder.RenameColumn(
                name: "QuestionBody",
                table: "Questions",
                newName: "Difficulty");

            migrationBuilder.RenameColumn(
                name: "QuestionID",
                table: "Questions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_QuizID",
                table: "Questions",
                newName: "IX_Questions_QuizId");

            migrationBuilder.RenameColumn(
                name: "QuestionID",
                table: "Answers",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "Answers",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "AnswerID",
                table: "Answers",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Answers_QuestionID",
                table: "Answers",
                newName: "IX_Answers_QuestionId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "TEXT",
                maxLength: 192,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Questions",
                type: "TEXT",
                maxLength: 384,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConfirmPassword = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 24, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
