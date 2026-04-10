using FluentMigrator;

namespace Sha.mzansilegal.Domain.Migrations
{
    [Migration(20260410160000)]
    public class M20260410160000 : Migration
    {
        public override void Up()
        {
            Alter.Column("title").OnTable("answer_citations").InSchema("mzl").AsString(500).Nullable();
            Alter.Column("chapter_title").OnTable("answer_citations").InSchema("mzl").AsString(500).Nullable();
            Alter.Column("section_number").OnTable("answer_citations").InSchema("mzl").AsString(100).Nullable();
            Alter.Column("section_title").OnTable("answer_citations").InSchema("mzl").AsString(500).Nullable();

            Alter.Column("file_name").OnTable("contract_analyses").InSchema("mzl").AsString(300).NotNullable();
            Alter.Column("clause_reference").OnTable("contract_flags").InSchema("mzl").AsString(int.MaxValue).Nullable();
        }

        public override void Down()
        {
        }
    }
}
