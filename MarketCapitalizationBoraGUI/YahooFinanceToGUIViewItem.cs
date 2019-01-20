using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MarketCapitalizationBoraGUI
{
    public class YahooFinanceToGUIViewItem
    {
        SortableBindingList<GUIViewItem> _guiViewItems { get; set; } 
        YahooFinanceDbContext _yahooFinanceDbContext { get; set; }

        public YahooFinanceToGUIViewItem()
        {
            _yahooFinanceDbContext = new YahooFinanceDbContext();
            _guiViewItems = new SortableBindingList<GUIViewItem>();
        }
        //全銘柄の日付範囲データを取得
        public SortableBindingList<GUIViewItem> GetBetweenDatetimeGUIViewItem(DateTime fromDatetime, DateTime toDatetime)
        {
            _guiViewItems.Clear();
            foreach(var codeList in _yahooFinanceDbContext.CodeLists)
            {
                var sw = new System.Diagnostics.Stopwatch();
                
                var codeAndDatetimeTradeIndexs = _yahooFinanceDbContext.TradeIndexs.Where(x => x.code == codeList.code).Where(x => fromDatetime <= x.date && x.date <= toDatetime);

                //最大時価総額のTradeIndex
                var IQueryHighCapTradeIndex = codeAndDatetimeTradeIndexs.Where(x => x.marketCapitalization == codeAndDatetimeTradeIndexs.Select(y => y.marketCapitalization).Max());
                //0以外の最低時価総額TradeIndex
                var IQueryLowCapTradeIndex = codeAndDatetimeTradeIndexs.Where(x => x.marketCapitalization == codeAndDatetimeTradeIndexs.Select(y => y.marketCapitalization).Where(cap => cap != 0).Min());
                //時価総額に値が入っていないなら飛ばす
                if (IQueryHighCapTradeIndex.FirstOrDefault() == null || IQueryLowCapTradeIndex.FirstOrDefault() == null)
                {
                    continue;
                }

                sw.Start();
                var testHigh = IQueryHighCapTradeIndex.FirstOrDefault();
                sw.Stop();
                System.Windows.Forms.MessageBox.Show($"テストHigh：  {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                var testLow = IQueryLowCapTradeIndex.FirstOrDefault();
                sw.Stop();
                System.Windows.Forms.MessageBox.Show($"テストLow：  {sw.Elapsed}");
                sw.Reset();

                sw.Start();
                _guiViewItems.Add(
                    new GUIViewItem
                    {
                        Code = codeList.code,
                        Name = codeList.name,
                        HighCap = IQueryHighCapTradeIndex.FirstOrDefault().marketCapitalization,
                        HighCapDatetime = IQueryHighCapTradeIndex.FirstOrDefault().date,
                        LowCap = IQueryLowCapTradeIndex.FirstOrDefault().marketCapitalization,
                        LowCapDatetime = IQueryLowCapTradeIndex.FirstOrDefault().date,
                    }
                );
                sw.Stop();
                System.Windows.Forms.MessageBox.Show($"メインAdd：  {sw.Elapsed}");
                sw.Reset();
            }            
            return _guiViewItems;
        }
    }
}
