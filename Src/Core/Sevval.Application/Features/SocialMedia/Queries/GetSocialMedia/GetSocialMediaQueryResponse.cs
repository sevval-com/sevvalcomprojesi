namespace Sevval.Application.Features.SocialMedia.Queries.GetSocialMedia;

public class GetSocialMediaQueryResponse
{
    public SocialMediaPlatform Facebook { get; set; }
    public SocialMediaPlatform Instagram { get; set; }
    public SocialMediaPlatform Youtube { get; set; }
    public SocialMediaPlatform Google { get; set; }
    public SocialMediaPlatform TikTok { get; set; }
}

public class SocialMediaPlatform
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string ImagePath { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
