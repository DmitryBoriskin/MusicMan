using cms.dbModel.entity;
using System;

namespace cms.dbModel
{
    public abstract class abstract_cmsRepository
    {
        public abstract bool DomainSecurity(string Domain);

        // Настройки
        public abstract SettingsModel getSettings();
        public abstract bool updateSettings(SettingsModel Item, Guid UserId, string IP);

        //// Работа с логами
        //public abstract cmsLogModel[] getCmsUserLog(Guid UserId);
        //public abstract cmsLogModel[] getCmsPageLog(Guid PageId);
        //public abstract void insertLog(Guid UserId, string IP, string Action, Guid PageId, string Desc);

        public abstract statModel[] getStatistic(int DayCount);

        // Все пользователи портала
        public abstract bool check_user(Guid id);
        public abstract bool check_user(string email);
        //public abstract void check_usergroup(Guid id, string group, Guid UserId, string IP);

        public abstract Catalog_list[] getUsersList();
        public abstract UsersList getUsersList(FilterParams filtr);
        public abstract User getUser(Guid id);
        public abstract string[] getUserCtegory(Guid id);
        public abstract bool saveUser(Guid id, User Item, Guid UserId, string IP);
        public abstract bool deleteUser(Guid id, Guid UserId, string IP);

        public abstract void changePassword(Guid id, string Salt, string Hash, Guid UserId, string IP);

        public abstract Catalog_list[] getUsersGroupList();
        public abstract Catalog_list[] getUsersCategorys();


        public abstract WorkList getWorkList(FilterParams filtr);
        public abstract WorkModel getWork(Guid Id);

        public abstract bool creditWork(WorkModel workItem, Guid UserId, string IP);
        public abstract bool deleteWork(Guid Id, Guid UserId, string IP);

        public abstract Catalog_list[] getWorksTypes();

        public abstract GaleryModel getPhotoAlbom(Guid workId);
        public abstract PhotoModel getPhotoItem(Guid id);

        public abstract void addPhoto(PhotoModel photoItem);
        public abstract bool delPhotoItem(Guid id);
        public abstract bool sortingPhotos(Guid id, int num);

        public abstract BannerModel[] getSlider(FilterParams filtr);
        public abstract BannerModel getSlide(Guid id);
        public abstract bool creditSlider(BannerModel Item, Guid UserId, string IP);
        public abstract bool deleteSlide(Guid id);
        
        public abstract BannerModel[] getBannersList(FilterParams filtr);
        public abstract BannerModel getBanner(Guid id);
        public abstract bool creditBanner(BannerModel Item, Guid UserId, string IP);
        public abstract bool deleteBanner(Guid id);

        //public abstract UsersGroupModel getUsersGroup(string alias);

        //public abstract ResolutionsModel[] getGroupResolutions(string alias);

        //// Материалы
        //public abstract MaterialsList getMaterialsList(FilterParams filtr);
        //public abstract MaterialsModel getMaterial(Guid id);

        //public abstract bool insertCmsMaterial(MaterialsModel material);
        //public abstract bool updateCmsMaterial(MaterialsModel material);
        //public abstract bool deleteCmsMaterial(Guid id);
    }
}