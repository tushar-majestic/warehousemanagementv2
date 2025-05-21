using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class LabDBContext : DbContext
{
    public LabDBContext(DbContextOptions<LabDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<ColorCode> ColorCodes { get; set; }

    public virtual DbSet<DamagedItem> DamagedItems { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<DisbursementRequest> DisbursementRequests { get; set; }

    public virtual DbSet<HazardType> HazardTypes { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemCard> ItemCards { get; set; }

    public virtual DbSet<ItemCardBatch> ItemCardBatches { get; set; }

    public virtual DbSet<ItemGroup> ItemGroups { get; set; }
    public virtual DbSet<ItemCardViewModels> ItemCardViewModels { get; set; }

    public DbSet<ItemCardExtended> ItemCardExtended { get; set; }
    public DbSet<DeductionExtended> DeductionExtended { get; set; }


    public virtual DbSet<ItemGroup> ItemGroup { get; set; }

    public virtual DbSet<ItemInfoByStoreId> ItemInfoByStoreIds { get; set; }

    public virtual DbSet<ItemType> ItemTypes { get; set; }

    public virtual DbSet<MaterialRequest> MaterialRequests { get; set; }
    public virtual DbSet<DespensedItem> DespensedItems { get; set; }


    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<PrimaryKey> PrimaryKeys { get; set; }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<ReceivingItem> ReceivingItems { get; set; }
    public virtual DbSet<ShelveItem> ShelveItems { get; set; }
    public virtual DbSet<PendingDeduction> PendingDeductions { get; set; }


    public virtual DbSet<ReceivingReport> ReceivingReports { get; set; }

    public virtual DbSet<Requester> Requesters { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Shelf> Shelves { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreDataResult> StoreDataResults { get; set; }

    public virtual DbSet<StoreMovement> StoreMovements { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Supply> Supplies { get; set; }

    public virtual DbSet<Tablecolumn> Tablecolumns { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<UserGroupPrivilege> UserGroupPrivileges { get; set; }

    public virtual DbSet<VActivityLog> VActivityLogs { get; set; }
    public DbSet<ReturnRequest> ReturnRequests { get; set; }
    public DbSet<ReturnRequestItem> ReturnRequestItems { get; set; }
    public DbSet<StoreTypes> StoreTypes { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.ActivityLogId).HasName("PK_AC.ActivityLog");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AC.ActivityLog_AC.User_User_ID");
        });

        modelBuilder.Entity<ColorCode>(entity =>
        {
            entity.HasKey(e => e.CodeId).HasName("PK__ColorCod__D033FA864E829A30");

            entity.HasOne(d => d.Item).WithMany(p => p.ColorCodes).HasConstraintName("FK_ItemID");
        });

        modelBuilder.Entity<DamagedItem>(entity =>
        {
            entity.HasKey(e => e.DamagedId).HasName("PK__DAMAGED___5EE431278CE1F0BF");

            entity.HasOne(d => d.Item).WithMany(p => p.DamagedItems).HasConstraintName("FK__DAMAGED_I__ItemI__797309D9");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DId).HasName("PK__Destinat__76B8FF7D9D658B66");
        });

        modelBuilder.Entity<DisbursementRequest>(entity =>
        {
            entity.Property(e => e.DisbursementRequestId).ValueGeneratedNever();

            entity.HasOne(d => d.Store).WithMany(p => p.DisbursementRequests).HasConstraintName("FK_DisbursementRequest_StoreId");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(e => e.ItemId).ValueGeneratedNever();

            entity.HasOne(d => d.GroupCodeNavigation).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_ItemGroup");

            entity.HasOne(d => d.HazardTypeNameNavigation).WithMany(p => p.Items).HasConstraintName("FK_Item_ItemHazardType");

            entity.HasOne(d => d.ItemTypeCodeNavigation).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_ItemType");

            entity.HasOne(d => d.Supply).WithMany(p => p.Items).HasConstraintName("FK__Item__SupplyID__76969D2E");

            entity.HasOne(d => d.Unit).WithMany(p => p.Items)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_ItemUnit");
        });

        modelBuilder.Entity<ItemGroup>(entity =>
        {
            entity.HasKey(e => e.GroupCode).HasName("PK_ItemGroup_1");
        });

        modelBuilder.Entity<ItemType>(entity =>
        {
            entity.HasKey(e => e.ItemTypeCode).HasName("PK_ItemType_1");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Type).HasDefaultValue("");
        });

        modelBuilder.Entity<PrimaryKey>(entity =>
        {
            entity.HasKey(e => e.TableName).HasName("PK__PrimaryK__733652EFA4AA236C");
        });

        modelBuilder.Entity<ReceivingItem>(entity =>
        {
            entity.Property(e => e.Comments).HasDefaultValue("");
        });

        modelBuilder.Entity<ReceivingReport>(entity =>
        {
            entity.Property(e => e.AttachmentPath).HasDefaultValue("");
            entity.Property(e => e.BasedOnDocument).HasDefaultValue("");
            // entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.DocumentNumber).HasDefaultValue("");
            entity.Property(e => e.FiscalYear).HasDefaultValue("");
            entity.Property(e => e.ReceivingWarehouse).HasDefaultValue("");
            entity.Property(e => e.RecipientSector).HasDefaultValue("");
            entity.Property(e => e.SectorNumber).HasDefaultValue("");
        });

        modelBuilder.Entity<Requester>(entity =>
        {
            entity.HasKey(e => e.ReqId).HasName("PK__REQUESTE__06143B5B0B81E13C");

            entity.HasOne(d => d.Destination).WithMany(p => p.Requesters).HasConstraintName("FK__REQUESTER__DESTI__245D67DE");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__ROOMS__328639396C49D5E5");

            entity.HasOne(d => d.Store).WithMany(p => p.Rooms).HasConstraintName("FK__ROOMS__StoreId__71D1E811");
        });

        modelBuilder.Entity<Shelf>(entity =>
        {
            entity.HasKey(e => e.ShelfId).HasName("PK__SHELVES__DBD04F0726157D69");

            entity.HasOne(d => d.Room).WithMany(p => p.Shelves).HasConstraintName("FK__SHELVES__RoomId__74AE54BC");

            entity.HasOne(d => d.Store).WithMany(p => p.Shelves).HasConstraintName("FK__SHELVES__StoreId__75A278F5");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasOne(d => d.Item).WithMany(p => p.Storages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Storage_Item");

            entity.HasOne(d => d.Room).WithMany(p => p.Storages).HasConstraintName("FK__Storage__RoomId__7A672E12");

            entity.HasOne(d => d.Shelf).WithMany(p => p.Storages).HasConstraintName("FK__Storage__ShelfId__7B5B524B");

            entity.HasOne(d => d.Store).WithMany(p => p.Storages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Storage_Store");

            entity.HasOne(d => d.Supply).WithMany(p => p.Storages).HasConstraintName("FK_Storage_Supply");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.Property(e => e.StoreId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(1);
            entity.Property(e => e.StoreType).IsFixedLength();
        });

        modelBuilder.Entity<StoreMovement>(entity =>
        {
            entity.HasKey(e => e.StoreMovementId).HasName("PK__STORE_MO__610F133843FD3233");

            entity.HasOne(d => d.FromRoom).WithMany(p => p.StoreMovements).HasConstraintName("FK__STORE_MOV__FromR__7F2BE32F");

            entity.HasOne(d => d.FromShelf).WithMany(p => p.StoreMovements).HasConstraintName("FK__STORE_MOV__FromS__00200768");

            entity.HasOne(d => d.FromStore).WithMany(p => p.StoreMovementFromStores).HasConstraintName("FK__STORE_MOV__FromS__7E37BEF6");

            entity.HasOne(d => d.Item).WithMany(p => p.StoreMovements).HasConstraintName("FK__STORE_MOV__ItemI__02084FDA");

            entity.HasOne(d => d.ToStore).WithMany(p => p.StoreMovementToStores).HasConstraintName("FK__STORE_MOV__ToSto__01142BA1");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.SupplierId).ValueGeneratedNever();
            entity.Property(e => e.CoordinatorName).HasDefaultValue("");
        });

        modelBuilder.Entity<Supply>(entity =>
        {
            entity.Property(e => e.SupplyId).ValueGeneratedNever();

            entity.HasOne(d => d.Supplier).WithMany(p => p.Supplies)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Supply_Supplier");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.Property(e => e.ChemicalStatus).IsFixedLength();
            entity.Property(e => e.DocumentType).IsFixedLength();
            entity.Property(e => e.HazardType).IsFixedLength();
            entity.Property(e => e.UnitsMeasure).IsFixedLength();
            entity.Property(e => e.WarehouseType).IsFixedLength();

            entity.HasOne(d => d.GroupCodeNavigation).WithMany(p => p.Units)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Units_UnitGroup");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.EmpAffiliation).HasDefaultValue("");
            entity.Property(e => e.Lang).IsFixedLength();
            entity.Property(e => e.Password).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.UserGroup).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_UserGroup");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.Property(e => e.UserGroupId).ValueGeneratedNever();
        });

        modelBuilder.Entity<UserGroupPrivilege>(entity =>
        {
            entity.HasOne(d => d.Privilege).WithMany(p => p.UserGroupPrivileges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserGroup_Privileges_Privileges");

            entity.HasOne(d => d.UserGroup).WithMany(p => p.UserGroupPrivileges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserGroup_Privileges_UserGroup");
        });

        modelBuilder.Entity<VActivityLog>(entity =>
        {
            entity.ToView("vActivityLog");
        });

        modelBuilder.Entity<ReturnRequest>()
        .HasOne(r => r.Warehouse)
        .WithMany() // If Store does not have a collection of ReturnRequests
        .HasForeignKey(r => r.WarehouseId)
        .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a Store if used

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
