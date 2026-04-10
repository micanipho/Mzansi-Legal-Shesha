using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sha.mzansilegal.Domain.Domain
{
    public abstract class MzansiLegalEntityBase : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        [Column("id")]
        public override Guid Id { get; set; }

        [Column("creation_time")]
        public override DateTime CreationTime { get; set; }

        [Column("creator_user_id")]
        public override long? CreatorUserId { get; set; }

        [Column("last_modification_time")]
        public override DateTime? LastModificationTime { get; set; }

        [Column("last_modifier_user_id")]
        public override long? LastModifierUserId { get; set; }

        [Column("is_deleted")]
        public override bool IsDeleted { get; set; }

        [Column("deletion_time")]
        public override DateTime? DeletionTime { get; set; }

        [Column("deleter_user_id")]
        public override long? DeleterUserId { get; set; }

        [Column("tenant_id")]
        public virtual int? TenantId { get; set; }
    }
}
