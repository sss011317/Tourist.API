using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tourist.API.Helper
{
    public class PaginationList<T> : List<T>
    {
        public int TotalPages { get;private set; } //頁面總量
        public int TotalCount { get;private set; } //資料庫總量 
        public bool HasPrevious => CurrentPage > 1; //判斷是否有上一頁
        public bool HasNext => CurrentPage < TotalPages; //判斷是否有下一頁





        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationList(int totalCount,int currentPage, int pageSize , List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

         public async static Task<PaginationList<T>> CreateAsync(int currentPage, int pageSize, IQueryable<T> result)
        {
            var totalCount = await result.CountAsync();
            //pagination 分頁功能添加在最後，首先需要處理好資料再做分頁，不然會很混亂
            //1.skip 跳過一定量的資料
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            //2.以pageSize為標準顯示一定量的資料
            result = result.Take(pageSize);
            var items = await result.ToListAsync();

            return new PaginationList<T>(totalCount,currentPage, pageSize, items);
        }

    } 
}
