using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE FULLTEXT CATALOG MailSearch WITH ACCENT_SENSITIVITY = ON;

                CREATE FULLTEXT INDEX ON Emails(Subject, Body)
                    KEY INDEX PK_Emails
                    ON MailSearch
                    WITH STOPLIST = SYSTEM, CHANGE_TRACKING = AUTO;

                CREATE FULLTEXT INDEX ON Users(Email)
                    KEY INDEX PK_Users
                    ON MailSearch
                    WITH STOPLIST = SYSTEM, CHANGE_TRACKING = AUTO;
                """, suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FULLTEXT INDEX ON Emails;
                DROP FULLTEXT INDEX ON Users;
                DROP FULLTEXT CATALOG MailSearch;
                """, suppressTransaction: true);
        }
    }
}
