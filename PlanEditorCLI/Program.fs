module PlanEditorCLI.Program

open PlanEditor.Types
open PlanEditor.ConsoleEditor
open System

// Функция для создания примерных данных
let createSampleCourses() : Course list =
    [
        // Пример 1: Английский язык
        { emptyCourse with
            Code = "060139"
            RussianName = "Английский язык в сфере профессиональной коммуникации"
            EnglishName = "English for Professional Communication"
            Type = Base
            Implementations = [
                { emptyImplementation with
                    Semester = 1
                    LaborIntensity = 2
                    BlockCode = Disciplines
                    Competences = ["УК-4"]
                    MonitoringTypes = [Зачет]
                    WorkHours = parseWorkHours "0 0 2 32 0 0 0 0 2 0 0 36 0 0 58"
                    Realization = ""
                    Trajectory = "" }
            ]
            ElectivesBlock = [] }
        
        // Пример 2: Математическая логика
        { emptyCourse with
            Code = "031539"
            RussianName = "Дополнительные главы математической логики и теории алгоритмов"
            EnglishName = "Additional Chapters of Mathematical Logic and Algorithm Theory"
            Type = Base
            Implementations = [
                { emptyImplementation with
                    Semester = 1
                    LaborIntensity = 5
                    BlockCode = Disciplines
                    Competences = ["ОПК-4"; "ОПК-6"]
                    MonitoringTypes = [Экзамен]
                    WorkHours = parseWorkHours "15 0 2 15 0 0 0 0 2 30 0 76 0 40 19"
                    Realization = ""
                    Trajectory = "" }
            ]
            ElectivesBlock = [] }
        
        // Пример 3: Практика
        { emptyCourse with
            Code = "070911"
            RussianName = "Учебная (ознакомительная) практика"
            EnglishName = "Professional (Introductory) Training"
            Type = Base
            Implementations = [
                { emptyImplementation with
                    Semester = 1
                    LaborIntensity = 4
                    BlockCode = PracticalTraining
                    Competences = ["УК-2"; "УКМ-1"; "УКМ-4"]
                    MonitoringTypes = [Зачет]
                    WorkHours = parseWorkHours "4 12 0 0 0 0 0 2 70 16 40 0 0 22 0"
                    Realization = ""
                    Trajectory = "" }
            ]
            ElectivesBlock = [] }
    ]

// Главное меню
let showMainMenu() =
    printfn ""
    printfn "╔════════════════════════════════════════╗"
    printfn "║     РЕДАКТОР УЧЕБНОГО ПЛАНА СПбГУ     ║"
    printfn "╚════════════════════════════════════════╝"
    printfn ""
    printfn "Основное меню:"
    printfn "┌─────────────────────────────────────────┐"
    printfn "│ 1. Запустить интерактивный редактор     │"
    printfn "│ 2. Создать пример и сохранить в файл   │"
    printfn "│ 3. Загрузить из файла                  │"
    printfn "│ 4. Проверить автосохранение            │"
    printfn "│ 5. Выйти из программы                  │"
    printfn "└─────────────────────────────────────────┘"
    printf "\nВыбор: "
    Console.ReadLine().Trim()

// Основная функция программы
[<EntryPoint>]
let main argv =
    Console.Clear()
    Console.OutputEncoding <- Text.Encoding.UTF8
    Console.InputEncoding <- Text.Encoding.UTF8
    printfn "Добро пожаловать в Редактор учебного плана!"
    printfn "Версия 1.0 | Для СПбГУ | Программная инженерия"
    printfn ""
    
    try
        // Обработка аргументов командной строки
        if argv.Length > 0 then
            match argv.[0].ToLower() with
            | "--help" | "-h" ->
                printfn "Использование:"
                printfn "  dotnet run [-- <аргументы>]"
                printfn ""
                printfn "Аргументы командной строки:"
                printfn "  --help, -h            Показать эту справку"
                printfn "  --sample, -s          Создать пример учебного плана"
                printfn "  --load <файл>, -l     Загрузить из JSON файла"
                printfn "  --editor, -e          Запустить интерактивный редактор"
                printfn "  --interactive, -i     Интерактивный режим (по умолчанию)"
                printfn ""
                printfn "Примеры:"
                printfn "  dotnet run -- --sample"
                printfn "  dotnet run -- --load plan.json"
                printfn "  dotnet run -- --editor"
                0  // Убираем return
                
            | "--sample" | "-s" ->
                printfn "Создание примера учебного плана..."
                let sampleCourses = createSampleCourses()
                let filename = "sample_plan.json"
                if saveToFile filename sampleCourses then
                    printfn "✓ Пример сохранен в файл: %s" filename
                    printfn "  Создано %d курсов." sampleCourses.Length
                else
                    printfn "✗ Не удалось сохранить пример."
                0  // Убираем return
                
            | "--load" | "-l" when argv.Length > 1 ->
                let filename = argv.[1]
                printfn "Загрузка из файла: %s" filename
                let courses = loadFromFile filename
                if not (List.isEmpty courses) then
                    printfn "✓ Загружено %d курсов из %s" courses.Length filename
                    printf "\nЗапустить интерактивный редактор? (y/n): "
                    if Console.ReadLine().ToLower() = "y" then
                        let editor = InteractiveEditor()
                        editor.StartEditor courses |> ignore
                else
                    printfn "✗ Не удалось загрузить файл или файл пуст."
                0  // Убираем return
                
            | "--editor" | "-e" ->
                printfn "Запуск интерактивного редактора..."
                let editor = InteractiveEditor()
                editor.StartEditor [] |> ignore
                0  // Убираем return
                
            | _ ->
                printfn "Неизвестный аргумент: %s" argv.[0]
                printfn "Используйте --help для справки."
                1  // Убираем return
        
        // Интерактивный режим (без аргументов)
        else
            let mutable continueLoop = true
            
            while continueLoop do
                match showMainMenu() with
                | "1" ->
                    // Запуск интерактивного редактора
                    printfn "\nЗапуск интерактивного редактора..."
                    let editor = InteractiveEditor()
                    let _ = editor.StartEditor []
                    printfn "\nРедактор завершил работу."
                    
                | "2" ->
                    // Создать пример
                    printfn "\nСоздание примера учебного плана..."
                    let sampleCourses = createSampleCourses()
                    printfn "✓ Создано %d примерных курсов:" sampleCourses.Length
                    
                    sampleCourses |> List.iteri (fun i course ->
                        printfn "  %d. [%s] %s" (i+1) course.Code course.RussianName)
                    
                    printf "\nСохранить в файл? (y/n): "
                    if Console.ReadLine().ToLower() = "y" then
                        printf "Имя файла [sample_plan.json]: "
                        let filename = 
                            let input = Console.ReadLine()
                            if String.IsNullOrWhiteSpace(input) then "sample_plan.json" else input
                        
                        if saveToFile filename sampleCourses then
                            printfn "✓ Сохранено в %s" filename
                        else
                            printfn "✗ Не удалось сохранить файл."
                    
                | "3" ->
                    // Загрузить из файла
                    printf "\nВведите имя файла для загрузки: "
                    let filename = Console.ReadLine()
                    
                    if System.IO.File.Exists(filename) then
                        let courses = loadFromFile filename
                        if not (List.isEmpty courses) then
                            printfn "✓ Загружено %d курсов из %s:" courses.Length filename
                            courses |> List.iteri (fun i course ->
                                printfn "  %d. [%s] %s" (i+1) course.Code course.RussianName)
                            
                            printf "\nХотите отредактировать загруженные курсы? (y/n): "
                            if Console.ReadLine().ToLower() = "y" then
                                let editor = InteractiveEditor()
                                editor.StartEditor courses |> ignore
                        else
                            printfn "✗ Файл пуст или поврежден."
                    else
                        printfn "✗ Файл не найден: %s" filename
                    
                | "4" ->
                    // Проверить автосохранение
                    let autosaveFile = "autosave.json"
                    if System.IO.File.Exists(autosaveFile) then
                        printfn "\nНайдено автосохранение: %s" autosaveFile
                        printf "Загрузить автосохранение? (y/n): "
                        if Console.ReadLine().ToLower() = "y" then
                            let autosaveCourses = loadFromFile autosaveFile
                            if not (List.isEmpty autosaveCourses) then
                                printfn "✓ Загружено %d курсов из автосохранения:" autosaveCourses.Length
                                autosaveCourses |> List.iteri (fun i course ->
                                    printfn "  %d. [%s] %s" (i+1) course.Code course.RussianName)
                                
                                printf "\nХотите отредактировать эти курсы? (y/n): "
                                if Console.ReadLine().ToLower() = "y" then
                                    let editor = InteractiveEditor()
                                    editor.StartEditor autosaveCourses |> ignore
                            else
                                printfn "✗ Автосохранение пусто."
                    else
                        printfn "\nАвтосохранение не найдено."
                    
                | "5" ->
                    // Выход
                    printfn "\nЗавершение работы..."
                    printfn "Спасибо за использование Редактора учебного плана!"
                    printfn "До свидания!"
                    continueLoop <- false
                    
                | _ ->
                    printfn "\n✗ Неизвестная команда. Попробуйте снова."
            
            0  // Успешный выход
    with
    | ex ->
        printfn "\n╔════════════════════════════════════════╗"
        printfn "║           ОШИБКА ПРОГРАММЫ             ║"
        printfn "╚════════════════════════════════════════╝"
        printfn ""
        printfn "Произошла ошибка: %s" ex.Message
        printfn ""
        printfn "Нажмите Enter для выхода..."
        Console.ReadLine() |> ignore
        1  // Код ошибки