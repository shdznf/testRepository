namespace SA
{
    /// <summary> Тип SQL команди </summary>
    public enum SqlCommand
    {
        /// <summary> Додати </summary>
        Insert,
        /// <summary> Оновити </summary>
        Update,
        /// <summary> Скопіювати </summary>
        Copy,
        /// <summary> Вилучити </summary>
        Delete,
        /// <summary> Оновити без питать </summary>
        SureUpdate
    };

    /// <summary> Дія не змінювати імя ніколи </summary>
    public struct SQLAction
    {
        /// <summary> Команда </summary>
        public SqlCommand cmd;
        /// <summary> Дані </summary>
        public object data;
    }

    /// <summary> Статуси правила </summary>
    public enum RuleStatuses
    {
        /// <summary> Створене </summary>
        Created = 1,
        /// <summary> Узгоджене </summary>
        Approved = 3,
        /// <summary> Заблоковане </summary>
        Blocked = 4
    }

    /// <summary> Операції які буду виконувати </summary>
    public enum Operations
    {
        /// <summary> Оновлення правила </summary>
        AccountingRuleUpdate,
        /// <summary> Вилучення(блокування) правила </summary>
        AccountingRuleDelete,
        /// <summary> Підтвердження правила </summary>
        AccountingRuleConfirm,
        /// <summary> Привязка  правила до бізнесу</summary>
        AccountingRuleUpdateBeLinks,
        /// <summary> Завантаження привязок правил до бізнесів</summary>
        AccountingRuleLoadBeLinks,
        /// <summary> Занесення на продуктив </summary>
        AccountingRuleToProduction,
        /// <summary> Занесено на продуктив </summary>
        AccountingRuleApproved
    }

    /// <summary> Нумератор таблиць які хешую </summary>
    public enum MdTable
    {
        /// <summary> Типи сум для генератора проводок </summary>
        DimEntrySums = 0,
    };

    /// <summary> Цвета редактора </summary>
    public enum EditorProperties
    {
        /// <summary> Пусто </summary>
        Empty = 0,
        /// <summary> Для розмальовки колонок редагування </summary>
        EditHeaderColumnColor = 1,
        /// <summary> Для розмальовки колонок редагування </summary>
        EditColumnColor = 2,
        /// <summary> Колонки необхідні для заповнення в редакторі</summary>
        EditHeaderMandatoryColumnColor = 3,
        /// <summary> Відображення </summary>
        UseModalForm = 4,
    };
}
//test//1
//1
