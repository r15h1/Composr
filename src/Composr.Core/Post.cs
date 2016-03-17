﻿using System;

namespace Composr.Core
{
    /// <summary>
    /// represents a blog post
    /// </summary>
    public class Post: IComposrEntity
    {
        public Post(Blog blog)
        {
            if (blog== null) throw new ArgumentNullException();
            if(!blog.Id.HasValue || blog.Id.Value <= 0) throw new ArgumentException("Invalid blog");

            Blog = blog;
            Status = PostStatus.DRAFT;
        }

        int? postid;
        public int? Id
        {
            get { return postid; }
            set 
            {
                if (value.HasValue && value.Value <= 0) throw new ArgumentException("postid must be greater than zero");
                postid = value;
            }
        }

        public PostStatus Status { get; set; }

        public string Title { get; set; }

        public Blog Blog { get; private set; }

        public string Body { get; set; }

        /// <summary>
        /// uniform resource name - must be unique by blogid and language
        /// </summary>
        public string URN { get; set; }
    }
}
