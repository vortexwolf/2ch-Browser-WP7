using System;
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
            this.AddBoardFromString("app; мобильные приложения");
            this.AddBoardFromString("au; автомобили и транспорт");
            this.AddBoardFromString("bi; велосипеды");
            this.AddBoardFromString("biz; бизнес");
            this.AddBoardFromString("bo; книги");
            this.AddBoardFromString("c; комиксы и мультфильмы");
            this.AddBoardFromString("di; столовая");
            this.AddBoardFromString("em; другие страны и туризм");
            this.AddBoardFromString("ew; конец света");
            this.AddBoardFromString("fa; мода и стиль");
            this.AddBoardFromString("fiz; физкультура");
            this.AddBoardFromString("fl; иностранные языки");
            this.AddBoardFromString("ftb; футбол");
            this.AddBoardFromString("gd; gamedev");
            this.AddBoardFromString("hh; хип-хоп культура");
            this.AddBoardFromString("hi; история");
            this.AddBoardFromString("hw; железо");
            this.AddBoardFromString("me; медицина");
            this.AddBoardFromString("mg; магия");
            this.AddBoardFromString("mlp; my little pony");
            this.AddBoardFromString("mov; фильмы");
            this.AddBoardFromString("mo; мотоциклы");
            this.AddBoardFromString("mu; музыка");
            this.AddBoardFromString("ne; животные и природа");
            this.AddBoardFromString("pvc; коллекционные фигурки");
            this.AddBoardFromString("po; политика и новости");
            this.AddBoardFromString("pr; программирование");
            this.AddBoardFromString("psy; психология");
            this.AddBoardFromString("ra; радиотехника");
            this.AddBoardFromString("re; религия и философия");
            this.AddBoardFromString("s; программы");
            this.AddBoardFromString("sf; научная фантастика");
            this.AddBoardFromString("sci; наука");
            this.AddBoardFromString("sn; паранормальные явления");
            this.AddBoardFromString("sp; спорт");
            this.AddBoardFromString("spc; космос");
            this.AddBoardFromString("t; техника");
            this.AddBoardFromString("tr; транспорт и авиация");
            this.AddBoardFromString("tv; тв");
            this.AddBoardFromString("un; образование");
            this.AddBoardFromString("wh; warhammer");
            this.AddBoardFromString("wm; военная техника");
            this.AddBoardFromString("w; оружие"); 

            // games
            this.AddBoardFromString("bg; настольные игры");
            this.AddBoardFromString("cg; консоли");
            this.AddBoardFromString("gb; азартные игры");
            this.AddBoardFromString("mc; minecraft");
            this.AddBoardFromString("mmo; MMO");
            this.AddBoardFromString("vg; видеоигры");
            this.AddBoardFromString("wr; текстовые ролевые игры");
            this.AddBoardFromString("tes; the elder scrolls");

            // random
            this.AddBoardFromString("abu; абу");
            this.AddBoardFromString("b; бред");
            this.AddBoardFromString("d; обсуждение двача");
            this.AddBoardFromString("fag; фагготрия");
            this.AddBoardFromString("soc; общение");
            this.AddBoardFromString("r; просьбы");
            this.AddBoardFromString("int; international");
        }

        private void AddBoardFromString(string str)
        {
            this.Boards.Add(this.ParseBoardItem(str));
        }

        private BoardItemViewModel ParseBoardItem(string str)
        {
            string[] strs = str.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

            return new BoardItemViewModel() { Code = strs[0], Title = strs[1] };
        } 
    }
}
