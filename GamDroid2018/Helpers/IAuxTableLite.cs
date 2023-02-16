using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GamDroid2018.Models;

namespace GamDroid2018.Helpers
{
    public interface IAuxTableLite
    {
        /// <summary>
        /// Genera una anulación
        /// </summary>
        /// <param name="cancellationList"></param>
        void SetCancellation(List<Cancellation> cancellationList);
        /// <summary>
        /// Devuelve la lista de Anulaciones de GAM
        /// </summary>
        /// <returns></returns>
        List<CancellationDto> GetCancellationList();
        /// <summary>
        /// Almacena en GamDroid la lista de gastos de GAM
        /// </summary>
        /// <param name="expenseList"></param>
        void SetExpenses(List<Expense> expenseList);

        List<ExpenseDto> GetListExpenses();
        /// <summary>
        /// Devuelve la lista de gastos que no se han podido enviar al servidor
        /// </summary>
        /// <returns></returns>
        List<ReportExpense> GetPendingExpense();
        /// <summary>
        /// Almacena en la tabla los gastos que no se han podido enviar al servidor
        /// </summary>
        /// <param name="expense"></param>
        void SaveReportExpense(ReportExpense expense);
        /// <summary>
        /// Elimina los gastos pendinetes de enviar
        /// </summary>
        /// <param name="id"></param>
        void DeletePendingExpense(string id);

        void ClearData();
    }
}