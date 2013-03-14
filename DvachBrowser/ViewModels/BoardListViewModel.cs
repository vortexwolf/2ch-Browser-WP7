using System.Collections.Generic;

using DvachBrowser.Assets;

using GalaSoft.MvvmLight.Command;

namespace DvachBrowser.ViewModels
{
    public class BoardListViewModel
    {
        private readonly PageNavigationService _pageNavigationService;

        public BoardListViewModel(PageNavigationService pageNavigationService)
        {
            this._pageNavigationService = pageNavigationService;

            this.Boards = new List<BoardItemViewModel>();
            this.GoToTypedBoardCommand = new RelayCommand(this.GoToTypedBoard);

            this.FillBoardList();
        }

        public List<BoardItemViewModel> Boards { get; set; }

        public string TypedBoard { get; set; }

        public RelayCommand GoToTypedBoardCommand { get; set; }

        public void GoToBoard(string boardName)
        {
            string queryString = new QueryStringBuilder()
                    .Add(Constants.QueryStringBoard, boardName)
                    .Build();

            this._pageNavigationService.Navigate(Constants.ThreadListPageUri + queryString);
        }

        private void GoToTypedBoard()
        {
            if (string.IsNullOrEmpty(this.TypedBoard))
            {
                return;
            }

            this.GoToBoard(this.TypedBoard);
        }

        private void FillBoardList()
        {
            this.Boards.Add(new BoardItemViewModel() { Code = "b", Title = "бред" });
            this.Boards.Add(new BoardItemViewModel() { Code = "app", Title = "мобильные приложения" });
            this.Boards.Add(new BoardItemViewModel() { Code = "au", Title = "автомобили и транспорт" });
            this.Boards.Add(new BoardItemViewModel() { Code = "bi", Title = "велосипеды" });
            this.Boards.Add(new BoardItemViewModel() { Code = "biz", Title = "бизнес" });
            this.Boards.Add(new BoardItemViewModel() { Code = "bo", Title = "книги" });
            this.Boards.Add(new BoardItemViewModel() { Code = "c", Title = "комиксы и мультфильмы" });
            this.Boards.Add(new BoardItemViewModel() { Code = "di", Title = "столовая" });
            this.Boards.Add(new BoardItemViewModel() { Code = "em", Title = "другие страны и туризм" });
            this.Boards.Add(new BoardItemViewModel() { Code = "ew", Title = "конец света" });
            this.Boards.Add(new BoardItemViewModel() { Code = "fa", Title = "мода и стиль" });
            this.Boards.Add(new BoardItemViewModel() { Code = "fiz", Title = "физкультура" });
            this.Boards.Add(new BoardItemViewModel() { Code = "fl", Title = "иностранные языки" });
            this.Boards.Add(new BoardItemViewModel() { Code = "ftb", Title = "футбол" });
            this.Boards.Add(new BoardItemViewModel() { Code = "gd", Title = "gamedev" });
            this.Boards.Add(new BoardItemViewModel() { Code = "hh", Title = "хип-хоп культура" });
            this.Boards.Add(new BoardItemViewModel() { Code = "hi", Title = "история" });
            this.Boards.Add(new BoardItemViewModel() { Code = "hw", Title = "железо" });
            this.Boards.Add(new BoardItemViewModel() { Code = "me", Title = "медицина" });
            this.Boards.Add(new BoardItemViewModel() { Code = "mg", Title = "магия" });
            this.Boards.Add(new BoardItemViewModel() { Code = "mlp", Title = "my little pony" });
            this.Boards.Add(new BoardItemViewModel() { Code = "mov", Title = "фильмы" });
        }
    }
}
