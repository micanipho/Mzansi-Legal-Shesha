using Sha.mzansilegal.Domain.Enums;
using Shesha.Domain.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sha.mzansilegal.Domain.Domain
{
    [Entity(TypeShortAlias = "Mzl.Category")]
    [Table("categories", Schema = "mzl")]
    public class Category : MzansiLegalEntityBase
    {
        [Column("name")]
        public virtual string Name { get; set; } = string.Empty;

        [Column("icon")]
        public virtual string? Icon { get; set; }

        [Column("domain_lkp")]
        [ReferenceList("Mzl", "CategoryDomains")]
        public virtual RefListCategoryDomains Domain { get; set; }

        [Column("sort_order")]
        public virtual int SortOrder { get; set; }

        [Column("parent_id")]
        public virtual Category? Parent { get; set; }

        public virtual ICollection<LegalDocument> LegalDocuments { get; set; } = new List<LegalDocument>();
    }
}
