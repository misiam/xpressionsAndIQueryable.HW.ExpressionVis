# xpressionsAndIQueryable.HW.ExpressionVis
Создайте класс-трансформатор на основе ExpressionVisitor, выполняющий следующие 2 вида преобразований дерева выражений: - Замену выражений вида &lt;переменная> + 1 / &lt;переменная> - 1 на операции инкремента и декремента - Замену параметров, входящих в lambda-выражение, на константы (в качестве параметров такого преобразования передавать: - Исходное выражение - Список пар &lt;имя параметра: значение для замены> Для контроля полученное дерево выводить в консоль или смотреть результат под отладчиком, использую ExpressionTreeVisualizer, а также компилировать его и вызывать полученный метод.
