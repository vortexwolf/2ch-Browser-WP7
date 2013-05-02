using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DvachBrowser.Assets
{
    /// <summary>
    /// Temporary class which I created so that the list of posts is refreshed after the post is sent.
    /// </summary>
    public class AddPostStorage
    {
        public AddPostStorage()
        {
            this.ClearCurrentDraft();
        }

        public bool IsSentSuccessfully { get; set; }

        public string ThreadNumber { get; set; }

        public DraftPost Draft { get; set; }

        public void ClearCurrentDraft()
        {
            this.ThreadNumber = null;
            this.Draft = new DraftPost();
        }

        public class DraftPost
        {
            public string CurrentText { get; set; }

            public bool IsSage { get; set; }

            public bool HasAttachment { get; set; }

            public string AttachmentName { get; set; }

            public byte[] AttachedFileBytes { get; set; }
        }
    }
}
