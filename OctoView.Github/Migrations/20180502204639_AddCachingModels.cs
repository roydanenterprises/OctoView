using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OctoView.Github.Migrations
{
	public partial class AddCachingModels : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
					name: "CachedBranches",
					columns: table => new
					{
						Id = table.Column<int>(nullable: false)
									.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
						BranchName = table.Column<string>(nullable: true),
						RepositoryId = table.Column<int>(nullable: true),
						Url = table.Column<string>(nullable: true)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_CachedBranches", x => x.Id);
						table.ForeignKey(
											name: "FK_CachedBranches_Repositories_RepositoryId",
											column: x => x.RepositoryId,
											principalTable: "Repositories",
											principalColumn: "Id",
											onDelete: ReferentialAction.Restrict);
					});

			migrationBuilder.CreateTable(
					name: "CachedPulls",
					columns: table => new
					{
						Id = table.Column<int>(nullable: false)
									.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
						Assignee = table.Column<string>(nullable: true),
						BranchId = table.Column<int>(nullable: true),
						Name = table.Column<string>(nullable: true),
						Number = table.Column<int>(nullable: false),
						Status = table.Column<string>(nullable: true),
						Url = table.Column<string>(nullable: true)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_CachedPulls", x => x.Id);
						table.ForeignKey(
											name: "FK_CachedPulls_CachedBranches_BranchId",
											column: x => x.BranchId,
											principalTable: "CachedBranches",
											principalColumn: "Id",
											onDelete: ReferentialAction.Restrict);
					});

			migrationBuilder.CreateTable(
					name: "CachedReviews",
					columns: table => new
					{
						Id = table.Column<int>(nullable: false)
									.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
						AvatarUrl = table.Column<string>(nullable: true),
						PullId = table.Column<int>(nullable: true),
						Status = table.Column<string>(nullable: true),
						Url = table.Column<string>(nullable: true),
						UserName = table.Column<string>(nullable: true)
					},
					constraints: table =>
					{
						table.PrimaryKey("PK_CachedReviews", x => x.Id);
						table.ForeignKey(
											name: "FK_CachedReviews_CachedPulls_PullId",
											column: x => x.PullId,
											principalTable: "CachedPulls",
											principalColumn: "Id",
											onDelete: ReferentialAction.Restrict);
					});

			migrationBuilder.CreateIndex(
					name: "IX_CachedBranches_RepositoryId",
					table: "CachedBranches",
					column: "RepositoryId");

			migrationBuilder.CreateIndex(
					name: "IX_CachedPulls_BranchId",
					table: "CachedPulls",
					column: "BranchId");

			migrationBuilder.CreateIndex(
					name: "IX_CachedReviews_PullId",
					table: "CachedReviews",
					column: "PullId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
					name: "CachedReviews");

			migrationBuilder.DropTable(
					name: "CachedPulls");

			migrationBuilder.DropTable(
					name: "CachedBranches");
		}
	}
}
