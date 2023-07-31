namespace Leavetown.Client.Utilities.Settings
{
    public class WhiteLabelSettings
    {
        public string? WhiteLabelIdentifier { get; set; }
        public string? DefaultCurrencyCode { get; set; } = null;
        public string? InquiryFormEmailBusiness { get; set; } = null;
        public string? InquiryFormEmailCustomer { get; set; } = null;
        public string? GoogleTagManagerContainerId { get; set; } = null;
        public int? PropertyManagerId { get; set; } = null;
        public int BlogsPageItemCount { get; set; } = 20;
        
        public bool UseExternalMarkupForFooter { get; set; } = false;

        public string? NavAboutExternalUrl { get; set; } = null;
        public string? NavBlogExternalUrl { get; set; } = null;
        public string? NavContactUsExternalUrl { get; set; } = null;
        public string? NavDestinationsExternalUrl { get; set; } = null;
        public string? NavEventsExternalUrl { get; set; } = null;
        public string? NavFAQExternalUrl { get; set; } = null;
        public string? NavForOwnersExternalUrl { get; set; } = null;
        public string? NavGalleryExternalUrl { get; set; } = null;
        public string? NavHomeExternalUrl { get; set; } = null;
        public string? NavListWithUsExternalUrl { get; set; } = null;
        public string? NavLocalInfoExternalUrl { get; set; } = null;
        public string? NavOffersExternalUrl { get; set; } = null;
        public string? NavOtherPropertiesExternalUrl { get; set; } = null;
        public string? NavOwnerResourcesExternalUrl { get; set; } = null;
        public string? NavSearchExternalUrl { get; set; } = null;
        public string? NavPrivacyPolicyExternalUrl { get; set; } = null;
        public string? NavTermsAndConditionsExternalUrl { get; set; } = null;
        public string? NavUserProfileExternalUrl { get; set; } = null;

        public bool HeaderNavShowAboutLink { get; set; } = false;
        public bool HeaderNavShowBlogLink { get; set; } = false;
        public bool HeaderNavShowContactUsLink { get; set; } = true;
        public bool HeaderNavShowDestinationsLink { get; set; } = false;
        public bool HeaderNavShowEventsLink { get; set; } = false;
        public bool HeaderNavShowListWithUsLink { get; set; } = false;
        public bool HeaderNavShowGalleryLink { get; set; } = false;
        public bool HeaderNavShowHomeLink { get; set; } = false;
        public bool HeaderNavShowSearchLink { get; set; } = false;
        public bool HeaderNavShowLocalInfoLink { get; set; } = false;
        public bool HeaderNavShowOffersLink { get; set; } = false;
        public bool HeaderNavShowOtherPropertiesLink { get; set; } = false;
        public bool HeaderNavShowUserProfileLink { get; set; } = false;
        public string? LocalInfoPageRoute { get; set; }

        public bool FooterNavShowBlogLink { get; set; } = false;
        public bool FooterNavShowEventsLink { get; set; } = false;
        public bool FooterNavShowGalleryLink { get; set; } = false;
        public bool FooterNavShowLocalInfoLink { get; set; } = false;
        public bool FooterNavShowListWithUsLink { get; set; } = false;
        public bool FooterNavShowOffersLink { get; set; } = false;
        public bool FooterNavShowOtherPropertiesLink { get; set; } = false;
        public bool FooterNavShowJetstreamLink { get; set; } = false;
        
        public string? HomePageHeroImageLinkUrl { get; set; } = null;

        public string? HomePageHeroVideoUrl { get; set; } = null;
        public bool HomePageShowCarRentalSection { get; set; } = false;
        public bool HomePageShowExperienceSection { get; set; } = true;
        public bool HomePageShowListWithUsSection { get; set; } = false;
        public bool HomePageShowNewsletterSubscription { get; set; } = true;
        public bool AccommodationsPageShowMap { get; set; } = true;
        public bool ShowElfReviewWidget { get; set; } = false;
    }
}
