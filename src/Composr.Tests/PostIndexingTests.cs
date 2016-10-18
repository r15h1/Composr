using Composr.Core;
using Composr.Lib.Indexing;
using Composr.Lib.Util;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Composr.Tests
{
    public class PostIndexingTests
    {
        private Blog blog = new Blog() { Id = 1};
        private IList<Post> posts;
        private ISearchService searcher;

        public PostIndexingTests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true);            
            Settings.Config = builder.Build();

            //SeedData();
            //GenerateIndex();
            searcher = new SearchService();
        }       

        private void GenerateIndex()
        {
            ClearIndexDirectory(blog);
            new Lib.Indexing.IndexWriter().IndexPosts(posts, null);
        }

        private static void ClearIndexDirectory(Blog blog)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Settings.IndexDirectory))
                System.IO.File.Delete(file);
        }

        private void SeedData()
        {            
            posts = new List<Post>();

            Post p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "Scala is an object-oriented programming language for the Java Virtual Machine, a functional language and combines the best approaches to OO and functional programming.");
            p.Attributes.Add(PostAttributeKeys.Tags, "book,scala,programming");
            p.Body = "<span>This is an amazingly readable book on Scala and the authors have done an awesome job in creating an introduction which is both interesting and substantial.  In particular, as the language architects, the authors were able to give reasons for various tradeoffs as the language was developed, something very helpful for those trying to evaluate a new language.  The book is written in a nurturing style, providing snapshots and examples of features early on with a deeper look at the details later.  It means that readers can very quickly understand language concepts without having to deal with difficult details until later on when they are more comfortable with the language.  The writing was lucid and smooth with a little bit of sprinkled humour (although the authors will not be moving onto the comedy circuit).<br /><br />My only negative was that I would have hoped for a language reference section, but I am unwilling to knock off even one star in the rating for this omission.<br /><br />In summary, this book is a terrific introduction for anyone who is already comfortable with object oriented (OO) programming, and with Java in particular.  For someone trying to learn Scala as a first language it will probably be a difficult read as it assumes familiarity with basic OO concepts, skills and tradeoffs.  But for existing programmers interested in a thorough introduction to Scala, this book is hard to beat.</span>";
            p.DatePublished = new System.DateTime(2012, 06, 13);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "Programming in Scala";
            p.URN = "/programming-in-scala";
            posts.Add(p);

            p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "Organized around concepts and use cases, this thoroughly updated sixth edition provides intermediate and advanced programmers with a concise map of C# and .NET knowledge. Dive in and discover why this Nutshell guide is considered the definitive reference on C#.");
            p.Attributes.Add(PostAttributeKeys.Tags, "book,csharp,programming");
            p.Body = "<div>Let's be clear, this is a book on C# 6 and not .NET Framework 4.6.  On the cover it states the book covers .NET Framework 4.6 but it only gives a very general overview of it.  This isn't necessarily a bad thing because it means the lion share of this huge 1000 + page nutshell is dedicated to the C# language.  In addition to an overview section, many of the individual sections cover elements of .NET Framework 4.6 but not deeply.<br /><br />Other books that cover .NET in greater detail, in addition to covering C#, are far too big.  For example, the Apress book &#34;C# 6.0 and the .NET 4.6 Framework&#34; by Andrew Troelsen is a 1700+ page book that weighs 5.5 pounds!!  This O'Reilly Nutshell book goes deeper into C# than the aforementioned tome.  C# and .NET are both deep subjects that should be covered in separate books.<br /><br />One reason I often shy away from the O'Reilly 'Nutshell' series of books is because the pages are smaller and as a consequence, the font is also small.  It's not easy to read code when it's printed this small.<br /><br />This book also has a very handy pocket reference that's sold separately here on Amazon.  I keep the full length book at home and take pocket reference to work.</div>";
            p.DatePublished = new System.DateTime(2014, 02, 25);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "C# 6.0 in a Nutshell: The Definitive Reference";
            p.URN = "/c-sharp-in-a-nutshell";
            posts.Add(p);

            p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "This Oracle Press resource also covers some of Java's more advanced features, including multithreaded programming, generics, and Swing.");
            p.Attributes.Add(PostAttributeKeys.Tags, "book,java,programming");
            p.Body = "<div>Using this book as a stand alone in 1st year computer-science java programming. I have prior knowledge in Python and an entire course dedicated to python programming, so I'm writing this as a &#34;Novice programmer who does not know Java at all&#34;.<br /><br />Pros: Book comes with complete solutions and walk through, worded nicely. If you know even a basic language or basic concepts of programming, this book will send you sailing. Cheaper and more effective than our in-class textbook ($125!!)<br /><br />Cons: Does not go into the computer science details or in-depth theory, I am not sure if you will be able to actually learn the reasoning or theory behind something like a nested loop or sub-arrays within arrays. You'll learn how to do them for sure but I can't say if you will know how to use them in the future.<br /><br />Overall: Get this if you have done programming! This is a solid book and for the price point, I can't ask for more! Just beware, you will have issues if you do NOT know ANY programming at all - I would recommend taking an online course in Python/Java - it is very hard to learn effectively/debug effectively if you take a nosedive into Java + using a book as your only foundation!</div>";
            p.DatePublished = new System.DateTime(2011, 12, 02);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "Java: A Beginner's Guide, Sixth Edition";
            p.URN = "/java-a-beginners-guide";
            posts.Add(p);

            p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "18.0 Megapixel CMOS (APS-C) image sensor and high-performance DIGIC 4 Image Processor for excellent speed and quality.");
            p.Attributes.Add(PostAttributeKeys.Tags, "canon,electronics,camera");
            p.Body = "<div>Interms of a beginner and user-friendly camera, it's well worth its value.  I am able to take beautiful clear pictures indoor and outdoor when the settings are done correctly.  That mean practicing how to use aperature and shutter speed in manual mode, while keeping ISO low, to achieve the ideal quality picture.  I also recommend purchasing a good lense, like the 55-250mm IS STM which comes with a stabilizer and covers good distance, as almost all DSLR cameras are only as good as the lenses you give it.  The kit lense works fine if you shoot at a high shutter speed (bright outdoor days) and have steady still hands. So put an effort into studying what the different lenses out there could do for you.  It also comes with easy-to-use photo editing softwares to help correct lighting and coloration to your liking; hence I highly recommend taking the photos in high quality RAW format. Here are some other things to consider when wanting to purchase this camera:<br /><br />- There are not a lot of special features with this camera; this is the very basic of DSLR and ideal for beginners, like myself, to practice on.<br />- Although it's only able to take 3 continuous shots per second, a good clear photo of a moving objects could be better achieved by setting focus to AI Servo mode<br />-The audio mic for video recording is quite poor and user have very little option for remedy other than to use manual hand focus to minimize lense movement noise (STM lense is an option too) or turn it completely off</div>";
            p.DatePublished = new System.DateTime(2011, 11, 07);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "Canon EOS Rebel T5";
            p.URN = "/canon-eos-rebel-t5";
            posts.Add(p);

            p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "AUTO-VOX A118-C B40C Stealth Car Dashboard Camera Capacitor Edition Covert Mini Dash Cam Full 1080P HD video No Internal Battery 170°Super Wide Angle 6G Lens with G-sensor WDR Night Vision Loop Recording Bundle with 32 GB Micro SD Card");
            p.Attributes.Add(PostAttributeKeys.Tags, "dashcam,electronics,autovox");
            p.Body = "<div>Best overall Dash Cam I have ever used. Fast delivery from Canadian locations, not all the way from US or China! I had a couple of others before this one. This is by far the best in terms of form factor, easy of use, useful features, reasonable price. It's been almost a month with no issues whatsoever. It worked under very hot conditions (above 45c). The only reason I took one star off is that the operating temp is no lower than -10c which would not work well in most places in Canada and it is not stated in product description. If you always warm up your car before every trip, then this may not be a big issue, will see how it goes in Winter time.<br />Follow up: So far so good for this winter, didn't give me any trouble whatsoever.</div>";
            p.DatePublished = new System.DateTime(2015, 08, 01);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "AUTO-VOX A118-C B40C Stealth Car Dashboard Camera";
            p.URN = "/auto-vox-stealth-dashboard-camera";
            posts.Add(p);


            p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "Along with legendary Toyota quality, durability and reliability, the 2016 Toyota Corolla is exceptionally sleek, sophisticated and fun-to-drive");
            p.Attributes.Add(PostAttributeKeys.Tags, "car,toyota");
            p.Body = "<div>Most Corollas sold in the U.S. are built at Toyota's Blue Springs, Miss., plant or a plant in Cambridge, Ontario. A small number (about 5 percent this year) are imported from Japan. Production at the Mississippi plant began with the 2012 model year.</div>";
            p.DatePublished = new System.DateTime(2015, 08, 01);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "Toyota Corolla";
            p.URN = "/toyota-corolla";
            posts.Add(p);

            p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "The Toyota Tundra is a pickup truck manufactured in the U.S.A. by the Japanese manufacturer Toyota since May 1999");
            p.Attributes.Add(PostAttributeKeys.Tags, "truck,toyota");
            p.Body = "<div>Try as it might, the Tundra is not quite up to the challenge of the competition despite its bold styling and a handsomely finished interior. It’s available in a myriad of body styles, bed lengths, and rear- or four-wheel drive. Powertrains include either a standard 310-hp 4.6-liter V-8 or an optional 381-hp 5.7-liter V-8; both engines team up with a six-speed automatic transmission. Maximum towing rates at 10,500 pounds; maximum payload is 2060 pounds.</div>";
            p.DatePublished = new System.DateTime(2015, 08, 01);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "Toyota Tundra";
            p.URN = "/toyota-tundra";
            posts.Add(p);

            p = new Post(blog);
            p.Attributes.Add(PostAttributeKeys.MetaDescription, "The Honda Civic is a line of small cars manufactured by Honda. Originally a subcompact, the Civic has gone through several generational changes");
            p.Attributes.Add(PostAttributeKeys.Tags, "car,honda");
            p.Body = "<div>The first Civic was introduced in July 1972 as a two-door model,[2] followed by a three-door hatchback that September. With an 1169 cc transverse engine and front-wheel drive like the British Mini, the car provided good interior space despite overall small dimensions.[3] Initially gaining a reputation for being fuel-efficient, reliable, and environmentally friendly, later iterations have become known for performance and sportiness.</div>";
            p.DatePublished = new System.DateTime(2015, 08, 01);
            p.Status = PostStatus.PUBLISHED;
            p.Title = "Honda Civi";
            p.URN = "/honda-civic";
            posts.Add(p);

        }


        [Fact]
        public void SearchByTagsMustReturnTaggedPosts()
        {
            SearchCriteria criteria = new SearchCriteria()
            {
                BlogID = blog.Id.Value,
                Locale = blog.Locale.Value,
                Limit = 1000,
                SearchSortOrder = SearchSortOrder.BestMatch,
                Tags = "book",
                SearchType = SearchType.Default,
                Start = 0
            };

            var post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 3);

            criteria.Tags = "electronics";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 2);

            criteria.Tags = "programming";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 3);

            criteria.Tags = "programming java";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 3);

            criteria.SearchTerm = "java";
            criteria.Tags = "programming";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 2);

            criteria.SearchTerm = "toyota";
            criteria.Tags = "car";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 1);

            criteria.SearchTerm = "toyota";
            criteria.Tags = "car,truck";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 2);

            criteria.SearchTerm = null;
            criteria.Tags = "car";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 2);

            criteria.SearchTerm = null;
            criteria.Tags = "truck";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 1);

            criteria.SearchTerm = "civic";
            criteria.Tags = "car,truck";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 1);

            criteria.SearchTerm = "civic";
            criteria.Tags = "truck";
            post = searcher.Search(criteria);
            Assert.True(post.Hits.Count == 0);
        }

        [Fact]
        public void MoreLikeThis()
        {
            var criteria = new SearchCriteria { BlogID = 1, Locale = Locale.EN, DocumentId = 68, Limit = 4, SearchType = SearchType.MoreLikeThis};
            var list = searcher.GetMoreLikeThis(criteria);
        }

    }
}