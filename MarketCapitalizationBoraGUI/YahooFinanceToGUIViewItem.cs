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
        Dictionary<int, List<TradeIndex>> _codeTradeIndexDic { get; set; }

        public YahooFinanceToGUIViewItem()
        {
            _yahooFinanceDbContext = new YahooFinanceDbContext();
            _guiViewItems = new SortableBindingList<GUIViewItem>();
            _codeTradeIndexDic = new Dictionary<int, List<TradeIndex>>();
        }
        //全銘柄の日付範囲データを取得
        public SortableBindingList<GUIViewItem> GetBetweenDatetimeGUIViewItem(DateTime fromDatetime, DateTime toDatetime)
        {
            _guiViewItems.Clear();
            //TradeIndexを銘柄コード別に実体を分ける ※時価総額が0円の場合はここで弾く　暫定          
            foreach (var tradeIndex in _yahooFinanceDbContext.TradeIndexs.Where(x => fromDatetime <= x.date && x.date <= toDatetime && x.marketCapitalization != 0).ToList())
            {
                if(_codeTradeIndexDic.ContainsKey(tradeIndex.code))
                {
                    _codeTradeIndexDic[tradeIndex.code].Add(tradeIndex);
                }
                else
                {
                    _codeTradeIndexDic.Add(tradeIndex.code, new List<TradeIndex> { tradeIndex });
                }
            }
            //銘柄コード別にビューアイテムの作成
            foreach (var key in _codeTradeIndexDic.Keys)
            {
                //最大時価総額のTradeIndex
                var highCapTradeIndex = _codeTradeIndexDic[key].Where(x => x.marketCapitalization == _codeTradeIndexDic[key].Select(y => y.marketCapitalization).Max()).FirstOrDefault();
                //0以外の最低時価総額TradeIndex
                var lowCapTradeIndex = _codeTradeIndexDic[key].Where(x => x.marketCapitalization == _codeTradeIndexDic[key].Select(y => y.marketCapitalization).Where(cap => cap != 0).Min()).FirstOrDefault();
                //時価総額に値が入っていないなら飛ばす
                if (highCapTradeIndex == null || lowCapTradeIndex == null)
                {
                    continue;                    
                }                
                _guiViewItems.Add(
                    new GUIViewItem
                    {
                        Code = highCapTradeIndex.code,
                        Name = highCapTradeIndex.price.codeList.name,
                        HighCap = highCapTradeIndex.marketCapitalization,
                        HighCapDatetime = highCapTradeIndex.date,
                        LowCap = lowCapTradeIndex.marketCapitalization,
                        LowCapDatetime = lowCapTradeIndex.date,
                    }
                );
            }            
            return _guiViewItems;
        }
    }
}
