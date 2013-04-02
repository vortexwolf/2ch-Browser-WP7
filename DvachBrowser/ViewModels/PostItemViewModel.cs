using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using DvachBrowser.Assets;
using DvachBrowser.Assets.Resources;
using DvachBrowser.Models;

using GalaSoft.MvvmLight.Command;

namespace DvachBrowser.ViewModels
{
    public class PostItemViewModel : ThreadPostBaseViewModel
    {
        private static readonly Regex ReferenceRegex = new Regex("<a.+?>&gt;&gt;(\\d+)</a>");

        private readonly PostListViewModel _parent;

        public PostItemViewModel(PostListViewModel parent)
            : base(parent.BoardName)
        {
            this._parent = parent;

            this.NavigateLinkCommand = new RelayCommand<Hyperlink>(this.NavigateToLink);
            this.ShowRepliesCommand = new RelayCommand(this.ShowReplies);

            this.RefersTo = new List<long>();
            this.ReferencesFrom = new List<long>();
        }

        private int _index;

        public int Index
        {
            get { return this._index; }
            set
            {
                this._index = value;
                this.RaisePropertyChanged("Index");
            }
        }

        public List<long> RefersTo { get; private set; }

        public List<long> ReferencesFrom { get; private set; }

        public ICommand NavigateLinkCommand { get; set; }

        public ICommand ShowRepliesCommand { get; set; }

        private string _repliesMessage;

        public string RepliesMessage
        {
            get { return this._repliesMessage; }
            set
            {
                this._repliesMessage = value;
                this.RaisePropertyChanged("RepliesMessage");
            }
        }

        private bool _hasReplies;

        public bool HasReplies
        {
            get { return this._hasReplies; }
            set
            {
                this._hasReplies = value;
                this.RaisePropertyChanged("HasReplies");
            }
        }

        public void MapModel(PostItemModel post, int index)
        {
            this.MapModel(post);
            this.ParseReferences();

            this.Index = index;
        }

        public void AddReferenceFrom(long number)
        {
            this.ReferencesFrom.Add(number);

            this.HasReplies = this.ReferencesFrom.Count > 0;
            this.RepliesMessage = string.Format(Strings.DataFormat_Replies, this.ReferencesFrom.Count);
        }

        private void ParseReferences()
        {
            var matches = ReferenceRegex.Matches(this.Comment);
            var refersTo = new List<long>();

            foreach (Match match in matches)
            {
                var refPostNumber = long.Parse(match.Groups[1].Value);

                refersTo.Add(refPostNumber);
            }

            this.RefersTo.AddRange(refersTo.Distinct());
        }

        private void NavigateToLink(Hyperlink link)
        {
            this._parent.NavigateToLink(this, link);
        }

        private void ShowReplies()
        {
            this._parent.ShowReplies(this);
        }
    }
}
