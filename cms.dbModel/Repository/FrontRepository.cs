using cms.dbModel.entity;
using System;

namespace cms.dbModel
{
    public abstract class abstract_FrontRepository
    {
        public abstract void insertLog(Guid UserId, string IP, string Action, Guid PageId, string Desc);

        public abstract AccountModel getAccount(Guid Id);
        public abstract AccountModel getAccount(string Login);
        public abstract AccountModel getAccount(string Login, Guid id);
        public abstract string[] getAccountcategory(Guid id);
        public abstract string[] getAccountcategoryName(Guid id);
        public abstract bool createAccount(AccountModel Item, string IP);
        public abstract bool updateAccount(AccountModel Item, string IP);

        public abstract bool setAccountVK(AccountModel Item, string IP);
        public abstract bool setAccountFacebook(AccountModel Item, string IP);

        public abstract void SuccessLogin(Guid id, string IP);
        public abstract int FailedLogin(Guid id, string IP);
        public abstract void setRestorePassCode(Guid id, Guid Code, string IP);
        public abstract bool ConfirmMail(Guid Code);
        public abstract void changePasswordUser(Guid id, string NewSalt, string NewHash, string IP);

        public abstract WorkList getWorkList(FilterParams filtr);
        public abstract WorkModel getWork(Guid Id);
        public abstract bool createWork(WorkModel workItem, string IP);
        public abstract bool deleteWork(Guid Id, string IP);
        public abstract GaleryModel getPhotoAlbom(Guid workId);
        public abstract void addPhoto(PhotoModel photoItem);

        public abstract bool checkLike(Guid UserId, Guid WorkId);
        public abstract int getLikes(Guid WorkId);
        public abstract int setLike(Guid UserId, Guid WorkId);

        public abstract BannerModel[] getSlider();
        public abstract BannerModel[] getBannersList(string sectionId);

        public abstract MaterialsList getMaterialsList(int Size);

        public abstract MaterialsList getMaterialsList(FilterParams filtr);
        //public abstract MaterialsModel getMaterial(int year, int manth, int day, string alias);

        public abstract Catalog_list[] getUsersGroupList();
        public abstract UsersList getUsersList(FilterParams filtr);
        //public abstract UsersModel getUser(string pageId);

        public abstract SettingsModel getSettings();
        public abstract SettingsModel getContacts();
    }
}