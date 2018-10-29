using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Storage;
using Windows.Storage.Streams;

namespace AsusRouterApp.Class
{
    public class ContractUtils
    {
        public static PinnedContactManager pinnedContactManager = PinnedContactManager.GetDefault();

        public static async Task<(bool res,Contact contact)> AddRouter(string mac,string name)
        {
            try
            {
                var list = await GetContactList();
                if (list == null) return (false, null);
                Contact contact = new Contact();
                contact.FirstName=name;
                var file=await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/images/router.png"));
                contact.SourceDisplayPicture= RandomAccessStreamReference.CreateFromFile(file);
                contact.Thumbnail = RandomAccessStreamReference.CreateFromFile(file);
                string appid = Windows.ApplicationModel.Package.Current.Id.FamilyName+"!App";
                contact.ProviderProperties.Add("ContactPanelAppID", appid);
                await list.SaveContactAsync(contact);
                var annotation = new ContactAnnotation();
                annotation.ContactId = contact.Id;
                annotation.ProviderProperties.Add("ContactPanelAppID", appid);
                annotation.SupportedOperations = ContactAnnotationOperations.ContactProfile;
                var annotationList =await GetContactAnnotationList();
                var res =await annotationList.TrySaveAnnotationAsync(annotation);
                return (res, contact);
            }
            catch (Exception e)
            {
                return (false, null);
            }
        }

        public static async void PinContact(Contact contact)
        {
            await pinnedContactManager.RequestPinContactAsync(contact,PinnedContactSurface.Taskbar);
        }

        public static async void UnpinContact(Contact contact)
        {
            await pinnedContactManager.RequestUnpinContactAsync(contact,PinnedContactSurface.Taskbar);
        }

        private static async Task<ContactList> GetContactList()
        {
            ContactStore store = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
            if (null == store)
            {
                return null;
            }
            ContactList contactList;
            IReadOnlyList<ContactList> contactLists = await store.FindContactListsAsync();
            if (0 == contactLists.Count)
            {
                contactList = await store.CreateContactListAsync("AsusRouterList");
            }
            else
            {
                contactList = contactLists[0];
            }
            return contactList;
        }

        private static async Task<ContactAnnotationList> GetContactAnnotationList()
        {
            ContactAnnotationStore annotationStore = await ContactManager.RequestAnnotationStoreAsync(ContactAnnotationStoreAccessType.AppAnnotationsReadWrite);
            if (null == annotationStore)
            {
                return null;
            }
            ContactAnnotationList annotationList;
            IReadOnlyList<ContactAnnotationList> annotationLists = await annotationStore.FindAnnotationListsAsync();
            if (0 == annotationLists.Count)
            {
                annotationList = await annotationStore.CreateAnnotationListAsync();
            }
            else
            {
                annotationList = annotationLists[0];
            }
            return annotationList;
        }
    }
}
