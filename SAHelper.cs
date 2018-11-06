using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using FozzySystems.Proxy;
using FozzySystems.Controls;
using FozzySystems.Types;
using FozzySystems.Types.Contracts;
using System.Windows.Forms;
using DevExpress.Utils;
using FozzySystems.Utils;
using FozzySystems;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using FozzySystems.Reporting;
using FozzySystems.Helpers;
using SA.Classes;

namespace SA
{
    /// <summary> Делегат отримання таблиці даних </summary>
    public delegate void AsyncExecDelegate(DataTable dt, int returnValue);

    /// <summary> Делегат отримання таблиць даних </summary>
    public delegate void AsyncExecDsDelegate(DataSet ds, int returnValue);

    /// <summary> Клас параметрів для виклику допоміжних операцій </summary>
    public class AsyncExecArgs
    {
        /// <summary> Контрол для блокування </summary>
        public Control ChaiseControl;
        /// <summary> Делегат отримання таблиці даних </summary>
        public AsyncExecDelegate Callback;
        /// <summary> Делегат отримання таблиць даних </summary>
        public AsyncExecDsDelegate CallbackDs;
    }

    /// <summary> Клас-помічник </summary>
    public static class SaHelper
    {
        /// <summary> Таблиці які хешуються при завантаженні </summary>
        public static Dictionary<MdTable, DataTable> HashedTables = new Dictionary<MdTable, DataTable>
        {
            {MdTable.DimEntrySums, new DataTable("sums")},
        };

        /// <summary> Розмальовка редактору</summary>
        public static IDictionary<EditorProperties, Color> EditFormColors = new Dictionary<EditorProperties, Color>
        {
            {EditorProperties.Empty, Color.Empty},
            {EditorProperties.EditHeaderColumnColor, Color.LightBlue},
            {EditorProperties.EditColumnColor, Color.Aquamarine},
            {EditorProperties.EditHeaderMandatoryColumnColor, Color.LightPink}
        };

        /// <summary> Відображення редактору</summary>
        public static IDictionary<EditorProperties, int> EditFormProperties = new Dictionary<EditorProperties, int>
        {
            {EditorProperties.UseModalForm, 0},
        };

        /// <summary> Шаблони налаштувань гріда </summary>
        public static ICollection<UserGridTemplate> UserGridTemplates;

        /// <summary> Критичні помилки, коли працювати вже не можливо </summary>
        public static IDictionary<int, string> CriticalErrorDic = new Dictionary<int, string>();

        /// <summary> Колонки які переносити з основного документу в ручний</summary>
        public static ICollection<string> HashIncludedColumn = new Collection<string>();

        /// <summary> Налаштування редагування по операціям </summary>
        public static ICollection<OperationEditor> HashOperationEditor = new Collection<OperationEditor>();

        /// <summary> Налаштування по операціям </summary>
        public static ICollection<OperationByActions> HashOperationByActions = new Collection<OperationByActions>();

        /// <summary> Налаштування по додатковим витратам </summary>
        public static ObservableCollection<ExpenseType> HashExpenseTypes = new ObservableCollection<ExpenseType>();

        /// <summary> Довідники для відображення </summary>
        public static IDictionary<string, DictionaryMapping> DictMappings = new Dictionary<string, DictionaryMapping>();

        /// <summary> Нотатки </summary>
        public static IList<string> NotesHistory = new List<string>();

        /// <summary> Статусы правил </summary>
        public static Dictionary<string, string> HashSapStatuses = new Dictionary<string, string>()
        {
            {"-1", string.Empty},
            {"0", "Новый"},
            {"1", "Отправлен"},
            {"2", "Принят"},
            {"3", "Ошибка"},
            {"4", "Отправлен СТОРНО"},
            {"5", "Сторнирован"},
            {"6", "Ошибка сторно"},
        };

        /// <summary> Статусы правил </summary>
        public static Dictionary<string, string> HashRuleStatuses = new Dictionary<string, string>()
        {
            {"1", "Создано"},
            {"3", "Утверждено(активно)"},
            {"4", "Отключено"},
        };

        /// <summary> Основные действия по которым правила </summary>
        public static Dictionary<string, string> HashRuleAction = new Dictionary<string, string>()
        {
            {"1", "Начальные проводки при приеме из DWH"},
            {"2", "Предварительное проведение"},
            {"3", "Проведение"},
        };

        /// <summary> Статусы </summary>
        public static Dictionary<string, string> HashStatuses = new Dictionary<string, string>()
        {
            {"0", "Загружен из DWH"},
            {"1", "Новый"},
            {"2", "Предварительно проведен"},
            {"3", "Проведен"},
            {"4", "Ожидание сторнирования"},
            {"5", "Сторнирован"},
            {"6", "Ожидание перевода в предыдущий статус"},
        };

        /// <summary> Типы документов накладных </summary>
        public static Dictionary<string, string> HashTaxTypes = new Dictionary<string, string>()
        {
            {"1", "НН"},
            {"2", "КН"},
            {"3", "ГНН"},
            {"4", "ГКН"},
            {"5", "ННКД"},
        };

        /// <summary> Интерфейсы передачи </summary>
        public static Dictionary<int, string> HashSendInterfaces = new Dictionary<int, string>();

        /// <summary> Типы накладной </summary>
        public static ICollection<TaxInvoiceType> HashTaxInvoiceTypes = new Collection<TaxInvoiceType>();

        /// <summary> Таблиця, курсу валют за дату</summary>
        public static IDictionary<DateTime, DataTable> HashRates = new Dictionary<DateTime, DataTable>();

        /// <summary> Таблиця, курсу валют за дату</summary>
        public static IDictionary<int, string> HashProblems = new Dictionary<int, string>();

        /// <summary> Таблиця, причин списання</summary>
        public static Dictionary<int, string> HashWriteOffReasons = new Dictionary<int, string>();

        /// <summary> Ставки НДС(гібрид) </summary>
        public static TaxRates TaxRateGroups = new TaxRates();

        /// <summary> Статусы </summary>
        public static Dictionary<string, string> YesNoDic = new Dictionary<string, string>
        {
            {"1", "Да"},
            {"0", "Нет"},
        };

        /// <summary> Вид налога: </summary>
        public static Dictionary<string, string> OutInDic = new Dictionary<string, string>
        {
            {"0", "Входящ"},
            {"1", "Исходящ"},
        };

        /// <summary> Тип трансферта: (внутр. , внешний) </summary>
        public static Dictionary<string, string> InsideOutsideDic = new Dictionary<string, string>
        {
            {"0", "Внутренний"},
            {"1", "Внешний"},
            {"2", "Между филиалами \"Экспансии\""}
        };

        /// <summary> Для правил додаткова можливість  </summary>
        public static Dictionary<int, string> CancelEntriesAction = new Dictionary<int, string>
        {
            {4, "Отменить последние"},
            {5, "Сторнировать все"},
        };

        /// <summary> Тип правила (порядок не менять) соответсвует ЕditAction </summary>
        public static Dictionary<int, string> RulesTypes = new Dictionary<int, string>
        {
            {0, "Для артикулов"},
            {1, "Для дополнительных расходов"},
            {2, "Для корректировок по артикулам"},
        };

        /// <summary> Тип суммы(isHeaderType) </summary>
        public static Dictionary<int, string> SumsTypes = new Dictionary<int, string>
        {
            {0, "Строкам"},
            {1, "Шапке"},
            {2, "Доп.расходам"},
        };

        /// <summary> Состояние документа(guid+guidError) </summary>
        public static Dictionary<string, string> DocStates = new Dictionary<string, string>
        {
            {"0", "Документ обработан"},
            {"1", "Документ в обработке"},
            {"2", "Документ с ошибками"},
        };

        /// <summary> Учитывать количество </summary>
        public static Dictionary<int, string> AllowQuantityStates = new Dictionary<int, string>
        {
            {0, "Нет"},
            {1, "Да"},
            {2, "При сумме 0 нет"},
            {3, "При сумме 0 Д как есть"},
            {4, "При сумме 0 К как есть"},
        };

        /// <summary> Информация о сторно </summary>
        public static Dictionary<int, string> StornoStates = new Dictionary<int, string>
        {
            {0, "Ожидает сторнирования"},
            {1, "Сторнирован в САП"},
            {2, "Сторнирован в САП и в СУ"},
        };

        /// <summary> типи дат </summary>
        public static Dictionary<string, string> DateTypes = new Dictionary<string, string>
        {
            {"entire", "Проводки"},
            {"operation", "Операционная"},
            {"empty", "Не использовать"}
        };

        /// <summary> Cтатуси перевірки по МІ</summary>
        public static Dictionary<int, string> MiCheckStates = new Dictionary<int, string>
        {
            {101, "Загрузка Прихода.Загружен (новый)"},
            {102, "Загрузка Прихода.Загружен (повторно)"},
            {201, "Сверка РН.Проблема с количеством"},
            {202, "Сверка РН.Проблема с ценой"},
            {203, "Сверка РН.Проблема с количеством и ценой"},
            {204, "Сверка РН.Ожидание накладной"},
            {206, "Сверка РН.Сверена"},
            {301, "Сверка НН.Сводная НН"},
            {303, "Сверка НН.Сверена"},
            {401, "Сторно Прихода.Ожидание сторно"},
            {402, "Сторно Прихода.Сторнирован"}
        };

        /// <summary> Для створення правила у редакторі проводок </summary>
        public static EntryAttributes EntryAttributes;

        /// <summary> Правила у редакторі проводок </summary>
        public static AccountingEntryRules AccountingEntryRules;

        /// <summary> Клас конфігурації. Дані з бази </summary>
        public static SaConfiguration SaMainConfiguration = new SaConfiguration();

        /// <summary> Класс хелпер </summary>
        static SaHelper()
        {
            DictMappings.Add("transferTypeId", new DictionaryMapping(InsideOutsideDic));
            DictMappings.Add("statusNN", new DictionaryMapping(StornoStates));
            DictMappings.Add("isHeaderType", new DictionaryMapping(SumsTypes));
            DictMappings.Add("AllowQuantity", new DictionaryMapping(AllowQuantityStates));
            // Динамически загружаемые
            DictMappings.Add("taxCode", new DictionaryMapping(TaxRateGroups, "TaxCode", "TaxName"));
            DictMappings.Add(DocumentHelper.ExpenseTypeId, new DictionaryMapping(HashExpenseTypes,"ExpenseId","TypeCategory"));
            DictMappings.Add("writeOffReasonId", new DictionaryMapping(HashWriteOffReasons));
            DictMappings.Add(DocumentHelper.MiCheckState, new DictionaryMapping(MiCheckStates));
        }
        
        #region ExecuteAsyncDS

        /// <summary> Асинхронное выполнение операции. Враппер над FZCoreProxy.ExecuteAsync.
        /// При успешном завершении вызывается указанный делегат, которому передается набор таблиц из результата - DataSet </summary>
        /// <param name="operationName">Операция</param>
        /// <param name="chaiseControl">Контрол, на котором запустить вертелку</param>
        /// <param name="fs"></param>
        /// <param name="callback">Делегат, вызываемый при успешном завершении операции</param>
        /// <param name="timeOut"></param>
        internal static void ExecuteAsyncDs(string operationName, Control chaiseControl, FilterSet fs, AsyncExecDsDelegate callback, int timeOut = 0)
        {
            var execArgs = new AsyncExecArgs
            {
                ChaiseControl = chaiseControl,
                CallbackDs = callback
            };

            if(chaiseControl != null)
                ChaseControl.AddToControl(chaiseControl, "Загрузка...");

            FZCoreProxy.ExecuteAsync(
                chaiseControl,// Control parent
                callback_ExecuteAsyncDS,// CallbackDelegate callback
                operationName,// string operationName
                null,// string request
                fs,// FilterSet filter
                timeOut,// int commandTimeout
                execArgs// object userState
                );
        }

        /// <summary> Колбек </summary>
        private static void callback_ExecuteAsyncDS(IDefaultContract contract, object obj)
        {
            var execArgs = obj as AsyncExecArgs;
            try
            {
                if(contract != null && contract.errorCode != ErrorCodes.OK)
                    throw new Exception(contract.errorString);
                var c = contract as ExecuteContract;
                if(c == null)
                    throw new Exception(string.Format("Непонятный контракт. Ждали ExecuteContract, получили {0}", contract));
                var returnValue = c.RetValue;
                if(execArgs == null)
                    return;

                if(execArgs.ChaiseControl != null)
                    ChaseControl.RemoveFromControl(execArgs.ChaiseControl);

                if(execArgs.CallbackDs != null)
                    execArgs.CallbackDs(c.dataSet, returnValue);
            }
            catch(Exception ex)
            {
                MB.critical(null, "Ошибка", Ex.Message(ex));
            }
        }

        #endregion

        /// <summary> Загрузка в кеш данных </summary>
        public static void UpdateDocumentMd()
        {
            // Із бази де живуть документи
            ExecuteAsyncDs("SA.SA@loadDocMD", null, null, (ds, rc) =>
            {
                HashWriteOffReasons.Clear();
                HashTaxInvoiceTypes = new List<TaxInvoiceType>();
                HashProblems = new Dictionary<int, string>();
                CriticalErrorDic = new Dictionary<int, string>();
                HashIncludedColumn = new List<string>();
                HashOperationEditor = new List<OperationEditor>();
                HashOperationByActions = new List<OperationByActions>();
                try
                {
                    var table = 0;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashTaxInvoiceTypes, r => new TaxInvoiceType(r));

                    table = 1;
                    if(ds.Tables.Count > table && ds.Tables[table] != null && ds.Tables[table].Rows.Count == 1)
                        SaMainConfiguration = new SaConfiguration(new DataRowAdapter(ds.Tables[table].Rows[0]));

                    table = 2;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(CriticalErrorDic, r => new KeyValuePair<int, string>(r["errorId"].IsNull(0), string.Format("{0}", r["errorText"])));

                    table = 3;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashProblems, r => new KeyValuePair<int, string>(r["problemId"].IsNull(0), string.Format("{0}", r["problemText"])));

                    table = 4;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashIncludedColumn, r => string.Format("{0}", r["value"]));

                    table = 5;
                    if (ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashOperationEditor, r => new OperationEditor(r));

                    table = 6;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashOperationByActions, r => new OperationByActions(r));

                    table = 7;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashWriteOffReasons, r => new KeyValuePair<int, string>(r["writeOffReasonId"].IsNull(0), string.Format("{0}", r["writeOffReasonName"])));
                }
                catch(Exception ex)
                {
                    MB.error(ex);
                }
            });
        }

        /// <summary> Загрузка в кеш данных </summary>
        public static void UpdateEntriesMd()
        {
            // Із бази де живуть проводки
            ExecuteAsyncDs("SA.SA@loadMD", null, null, (ds, rc) =>
            {
                HashSendInterfaces=new Dictionary<int, string>();
                HashExpenseTypes.Clear();
                TaxRateGroups.Clear();
                try
                {
                    // 0-я таблица - виды сумм
                    var table = 0;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        HashedTables[MdTable.DimEntrySums] = ds.Tables[table];

                    // 1-я таблица - назви интерфейсів передачі
                    table = 1;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashSendInterfaces, r => new KeyValuePair<int, string>(Convert.ToInt32(r["sendId"]), string.Format("{0}", r["sendName"])));

                    table = 2;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(HashExpenseTypes, r => new ExpenseType(r));

                    table = 3;
                    if(ds.Tables.Count > table && ds.Tables[table] != null)
                        ds.Tables[table].ToCollection(TaxRateGroups, r => new TaxRate(r));
                }
                catch(Exception ex)
                {
                    MB.error(ex);
                }
            });
        }

        /// <summary> Загрузка в кеш аттрибутов для генератора проводок </summary>
        /// <param name="onDataReady">Иссполнить при загрузке</param>
        /// <param name="owner">Контрол</param>
        public static void UpdateMD_accountingRules(Action onDataReady = null,Control owner = null)
        {
            var filter = new FilterSet();
            if (FZCoreProxy.Session.IsOperationAvailable("SA.Forms.AccEntriesRules"))
                filter["rulePermissionExists"] = new FilterSetItem("rulePermissionExists", FilterType.Static, 1);

            ExecuteAsyncDs("SA.AccountingRules@load", owner, filter, (ds, rc) =>
            {
                try
                {
                    if(ds.Tables[0] != null)
                    {
                        var xmls = ds.Tables[0];
                        if(!string.IsNullOrEmpty(xmls.Rows[0]["@entryAttributes"].ToString()))
                            EntryAttributes = Serialization.Deserialize<EntryAttributes>(xmls.Rows[0]["@entryAttributes"].ToString());

                        if(!string.IsNullOrEmpty(xmls.Rows[0]["@accountingRules"].ToString()))
                            AccountingEntryRules = Serialization.Deserialize<AccountingEntryRules>(xmls.Rows[0]["@accountingRules"].ToString());
                    }
                    if(onDataReady != null)
                        onDataReady();
                }
                catch (Exception ex)
                {
                    MB.error(ex);
                }
            });
        }

        /// <summary> Вилучити правило </summary>
        /// <param name="rule"></param>
        public static void AccountingRuleRemove(AccountingEntryRule rule)
        {
            AccountingEntryRules.Remove(rule);
        }

        /// <summary> Получить текущую выбранную строку в гриде </summary>
        /// <param name="o">sender из событий. показывает на GridView</param>
        /// <returns>DataRow, соответствующий текущей выбранной строке в гриде</returns>
        public static DataRow GetCurrentDataRow(object o)
        {
            var view = o as GridView;
            if(view == null)
            {
                // возможно нам скормили GridControl?
                var gc = o as GridControl;
                if(gc != null)
                    view = gc.DefaultView as GridView;
            }
            if(view == null)
                return null;
            if(view.FocusedRowHandle < 0)
                return null;
            var dt = view.GridControl.DataSource as DataTable;
            if(dt == null)
                return null;
            var dataRowHandle = view.GetDataSourceRowIndex(view.FocusedRowHandle);
            return dt.Rows[dataRowHandle];
        }

        #region ExecuteAsync

        /// <summary> Асинхронное выполнение операции. Враппер над FZCoreProxy.ExecuteAsync. При успешном завершении вызывается указанный делегат. </summary>
        /// <param name="operationName">Операция</param>
        /// <param name="chaiseControl">Контрол, на котором запустить вертелку</param>
        /// <param name="fs">FilterSet</param>
        /// <param name="data">Данные для серилизации</param>
        /// <param name="callback">Делегат, вызываемый при успешном завершении операции</param>
        internal static void ExecuteAsync(string operationName, Control chaiseControl, FilterSet fs, object data, AsyncExecDelegate callback)
        {
            var execArgs = new AsyncExecArgs
            {
                ChaiseControl = chaiseControl,
                Callback = callback
            };

            if(chaiseControl != null)
                ChaseControl.AddToControl(chaiseControl, "Загрузка...");

            FZCoreProxy.ExecuteAsync
                (
                    chaiseControl,// Control parent
                    callback_ExecuteAsync,// CallbackDelegate callback
                    operationName,// string operationName
                    data == null ? null : Serialization.Serialize(data),// string request
                    fs,// FilterSet filter
                    0,// int commandTimeout
                    execArgs// object userState
                );
        }

        /// <summary> Колбек </summary>
        /// <param name="contract">Контракт</param>
        /// <param name="obj">щось</param>
        private static void callback_ExecuteAsync(IDefaultContract contract, object obj)
        {
            var execArgs = obj as AsyncExecArgs;
            try
            {
                if(contract != null && contract.errorCode != ErrorCodes.OK)
                    throw new Exception(contract.errorString);
                var c = contract as ExecuteContract;
                if(c == null)
                    throw new Exception(string.Format("Непонятный контракт. Ждали ExecuteContract, получили {0}", contract));
                var returnValue = c.RetValue;
                if(execArgs == null)
                    return;
                var dt = c.dataSet.Tables[0];

                if(execArgs.ChaiseControl != null)
                    ChaseControl.RemoveFromControl(execArgs.ChaiseControl);

                if(execArgs.Callback != null)
                    execArgs.Callback(dt, returnValue);
            }
            catch(Exception ex)
            {
                MB.critical(null, "Ошибка", Ex.Message(ex));
            }
        }

        #endregion

        private static DataTable GetDt(DefaultMessageContract c)
        {
            var reader = c.GetDataReader();
            if(reader.IsClosed)
                throw new Exception("Reader is closed");
            var dt = c.CreateTable(reader, "data");
            while(!reader.IsClosed)
            {
                dt.BeginLoadData();
                while(reader.Read())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    dt.LoadDataRow(values, true);
                }
                dt.EndLoadData();
            }
            reader.Close();
            return dt;
        }

        #region ExecuterReaderAsyncDT

        /// <summary> Аснихронное выполнение операции с потоковым чтением. Враппер над FZCoreProxy.ExecuteReaderAsync.
        /// При успешном завершении вызывается делегат с DataTable </summary>
        /// <param name="operationName">Операция</param>
        /// <param name="chaiseControl">Контрол, на котором запускать вертелку</param>
        /// <param name="fs">Фильтр</param>
        /// <param name="callback">Делегат, вызываемый при успешном завершении операции</param>
        internal static void ExecuteReaderAsyncDt(string operationName, Control chaiseControl, FilterSet fs, AsyncExecDelegate callback)
        {
            var execArgs = new AsyncExecArgs
            {
                ChaiseControl = chaiseControl,
                Callback = callback
            };

            if(chaiseControl != null)
                ChaseControl.AddToControl(chaiseControl, "Загрузка...");

            FZCoreProxy.ExecuteReaderAsync(
                null,// Control parent
                callback_ExecuteReaderAsyncDT,// CallbackDelegate callback
                operationName,// string operationName
                "",// string request
                fs ?? new FilterSet(),// FilterSet filter
                0,// command timeout
                execArgs);
        }

        /// <summary> Колбек </summary>
        private static void callback_ExecuteReaderAsyncDT(IDefaultContract contract, object obj)
        {
            try
            {
                if(contract.errorCode != ErrorCodes.OK)
                    throw new Exception(contract.errorString);
                var c = contract as ExecuteReaderContract;
                if(c == null)
                    throw new Exception("Ждали ExecuteReaderContract");

                var returnValue = c.ReturnValue;
                var execArgs = obj as AsyncExecArgs;

                using(var dt = GetDt(c))
                {
                    if(execArgs == null)
                        return;
                    if(execArgs.ChaiseControl != null)
                        ChaseControl.RemoveFromControl(execArgs.ChaiseControl);

                    if(execArgs.Callback != null)
                        execArgs.Callback(dt, returnValue);
                }
            }
            catch(Exception ex)
            {
                MB.critical(null, "Ошибка", Ex.Message(ex));
            }
        }

        #endregion

        #region staticResources

        /// <summary> Набор описаний фильтра по MasterData для отчета. Ключ - название фильтра (поле FilterDescription.filterName). </summary>
        internal static Dictionary<string, FilterDescription> FilterDescriptions = new Dictionary<string, FilterDescription>();

        /// <summary> Глобальный WaitControl, который задается извне. Например, используется при drill-down, чтобы не перерисовывать 
        /// несколько WaitControl при загрузке темплейтов и выполнении запроса. </summary>
        internal static void LoadTemplates(string tplName)
        {
            LoadTemplates(tplName, null);
        }

        /// <summary> Завантаження початкових налаштувань </summary>
        /// <param name="tplName"></param>
        /// <param name="waitingControl"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        internal static void LoadTemplates(string tplName, Control waitingControl)
        {
            var stopLoadTemplates = false;
            var connections = new List<Guid>();
            try
            {
                EventHandler stopLoad = (sender, e) =>
                {
                    if ((sender as Control) != null)
                        ((Control)sender).Enabled = false;
                    stopLoadTemplates = true;
                };

                var w = (waitingControl == null) ? null : WaitControl.Show(waitingControl, null);
                if (w != null)
                    w.CancelEvent += stopLoad;
                try
                {
                    if (w != null)
                    {
                        w.Title = "Инициализация";
                        w.Text = "Загрузка настроек";
                    }
                    var errorString = "";
                    var counter = 0;

                    var handlers = new ResourceManager.ResourceHandlers
                    {
                        OnLoadError = x => errorString += x,
                        DataHandler = (x, y) =>
                        {
                            if (w != null)
                                w.ProgressValue = w.ProgressTotal - counter;
                            counter--;
                        },
                    };

                    // Loading Report.xml
                    counter = 1;
                    if (w != null)
                        w.ProgressTotal = counter;
                    var reportResourceName = string.Format("{0}", tplName);
                    connections.Add(ResourceManager.GetResourceAsync(reportResourceName, handlers));

                    sys.WaitFor(() => counter == 0 || stopLoadTemplates);

                    if (!string.IsNullOrEmpty(errorString))
                        throw new Exception(errorString);

                    if (stopLoadTemplates)
                        throw new Exception("Загрузка настроек отчета прервана");

                    var reportResources = Serialization.Deserialize<ReportResources>(Encoding.UTF8.GetString(ResourceManager.GetResource(reportResourceName)));

                    // Loading all templates
                    counter = reportResources.Count;

                    if (w != null)
                        w.ProgressTotal = counter;

                    for (var i = 0; i < reportResources.Count; i++)
                        connections.Add(ResourceManager.GetResourceAsync(reportResources[i].Value, handlers));

                    sys.WaitFor(() => counter == 0 || stopLoadTemplates);

                    if (!string.IsNullOrEmpty(errorString))
                        throw new Exception(errorString);

                    if (stopLoadTemplates)
                        throw new Exception("Загрузка настроек отчета прервана");

                    // All resources are loaded. Parsing Report.xml
                    //var resParamDict=reportResources.GetParamsDictionary();

                    FilterDescriptions.Clear();
                    for (var i = 0; i < reportResources.Count; i++)
                    {
                        var resourceName = reportResources[i].Value;
                        switch (reportResources[i].type)
                        {
                            case ReportResourceType.FilterDescription:
                                var fd = Serialization.Deserialize<FilterDescription>(Encoding.UTF8.GetString(ResourceManager.GetResource(resourceName)));
                                if (fd.extensions != null && fd.extensions.Length > 0)
                                    int.TryParse(reportResources[i].@default, out fd.extensions[0].@default);
                                if (!FilterDescriptions.ContainsKey(fd.filterName))
                                    FilterDescriptions.Add(fd.filterName, fd);
                                break;
                            default:
                                throw new NotImplementedException(reportResources[i].type + " is not implemented");
                        }
                    }
                }
                finally
                {
                    if (w != null)
                        w.Dispose();
                }
            }
            catch (Exception)
            {
                foreach (var guid in connections)
                    FZCoreProxy.AbortConnection(guid);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Загружает Layout для грида ReportFormGrid из xml-файла ресурсов (Fozzy)
        /// </summary>
        /// <param name="gc">грид</param>
        /// <param name="xmlFileName">имя файла ресурсов</param>
        /// <param name="setAllColumnsAsReadOnly">Если "истина", то все столбцы грида становятся readonly (часто нужно для отчётов Faust)</param>
        public static void LoadXMLSchemaForGrid(FozzySystems.Reporting.Controls.ReportFormGrid gc, String xmlFileName,
            bool setAllColumnsAsReadOnly)
        {
            ResourceManager.ResourceHandlers handlers = new ResourceManager.ResourceHandlers()
            {
                DataHandler = (x, y) =>
                {
                    if (x == null)
                        throw new Exception(String.Format("Ресурс не найден: {0}", xmlFileName));

                    Stream strm = new MemoryStream(x);

                    gc.DefaultView.BeginDataUpdate();
                    (gc as GridControl).DefaultView.RestoreLayoutFromStream(strm, OptionsLayoutBase.FullLayout);
                    // загружаем Layout грида из потока
                    gc.DefaultView.EndDataUpdate();

                    if (setAllColumnsAsReadOnly) // запрещаем редактирование столбцов, если задано
                        foreach (GridColumn c in (gc.DefaultView as GridView).Columns)
                        {
                            c.OptionsColumn.AllowEdit = false;
                        }
                }
            };
            ResourceManager.GetResourceAsync(xmlFileName, handlers);
        }

    }
}
