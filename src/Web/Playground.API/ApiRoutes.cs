namespace Playground.API
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        //public static class Posts
        //{
        //    private const string PostsControllerBase = Base + "/posts";

        //    public const string GetAll = PostsControllerBase;
        //    public const string Update = PostsControllerBase + "/{postId}";
        //    public const string Delete = PostsControllerBase + "/{postId}";
        //    public const string Get = PostsControllerBase + "/{postId}";
        //    public const string Create = PostsControllerBase;
        //}

        public static class Identity
        {
            private const string IdentityControlerBase = Base + "/identity";

            public const string Login = IdentityControlerBase + "/login";
            public const string Register = IdentityControlerBase + "/register";
            public const string Refresh = IdentityControlerBase + "/refresh";
        }

        //public static class Tags
        //{
        //    private const string TagsControlerBase = Base + "/tags";

        //    public const string GetAll = TagsControlerBase;
        //    public const string Delete = TagsControlerBase + "/{tagName}";
        //    public const string Get = TagsControlerBase + "/{tagName}";
        //    public const string Create = TagsControlerBase;
        //}
    }
}