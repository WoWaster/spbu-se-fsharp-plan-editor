module PlanEditor.ExcelExport

open PlanEditor.Types
open ClosedXML.Excel

let distinctSortMapCommaConcat f (items: 'a list) : string =
    items |> List.distinct |> List.sort |> List.map f |> String.concat ", "

let distinctSortCommaConcat items = distinctSortMapCommaConcat id items

type CompetencyDto = { Code: string; Description: string }

let fgosBlockCodeToString (code: FgosBlockCode) : string =
    match code with
    | Disciplines -> "Блок.1.дисц"
    | PracticalTraining -> "Блок.2.прки"
    | Gia -> "Блок.3.гиа"

// Функция для определения типа блока на основе названия курса
let detectBlockCode (courseName: string) (implName: string option) : FgosBlockCode =
    let text = 
        match implName with
        | Some name -> courseName + " " + name
        | None -> courseName
    
    let lowerText = text.ToLowerInvariant()
    
    // Проверяем на практику
    if lowerText.Contains("практика") || 
       lowerText.Contains("учебная практика") ||
       lowerText.Contains("производственная практика") ||
       lowerText.Contains("научно-исследовательская практика") ||
       lowerText.Contains("технологическая практика") ||
       lowerText.Contains("преддипломная практика") then
        PracticalTraining
    // Проверяем на ГИА (Государственная итоговая аттестация)
    elif lowerText.Contains("гиа") ||
         lowerText.Contains("государственная итоговая аттестация") ||
         lowerText.Contains("выпускная квалификационная работа") ||
         lowerText.Contains("диплом") ||
         lowerText.Contains("аттестационная работа") then
        Gia
    else
        Disciplines  // По умолчанию - дисциплина

type BaseDisciplineDto =
    { FgosCodeBlock: string
      Workload: int
      Competencies: string
      FullName: string
      AssessmentForms: string
      Lectures: int
      Seminars: int
      Consultations: int
      PracticalClasses: int
      LaboratoryWorks: int
      ControlWorks: int
      Colloquiums: int
      CurrentAssessmentClassroom: int
      IntermediateAssessmentClassroom: int
      UnderInstructorSupervision: int
      InInstructorPresence: int
      UsingMaterials: int
      CurrentAssessmentIndependent: int
      IntermediateAssessmentIndependent: int
      InteractiveHours: int }

type SemesterDto =
    { Number: int
      BaseDisciplines: BaseDisciplineDto list }

let toBaseDisciplineDto (course: Course) (impl: Implementation) : BaseDisciplineDto =
    let fullName =
        sprintf "[%s] %s\n%s" course.Code course.RussianName course.EnglishName
        + (if impl.Realization <> "" then sprintf " (%s)" impl.Realization else "")
        + (if impl.Trajectory <> "" then sprintf ", %s" impl.Trajectory else "")

    let assessmentForms = 
        if System.String.IsNullOrWhiteSpace(impl.MonitoringTypes) then
            ""
        else
            match impl.MonitoringTypes.ToLower() with
            | "экзамен" -> "экзамен"
            | "зачет" | "зачёт" -> "зачёт"
            | "аттестационноеиспытание" | "аттестация" -> "аттестационное испытание"
            | other -> other

    let blockCode = 
        match box impl.BlockCode with
        | null -> 
            detectBlockCode course.RussianName (Some impl.Realization)
        | _ -> impl.BlockCode

    { FgosCodeBlock = fgosBlockCodeToString blockCode
      Workload = impl.LaborIntensity
      Competencies = impl.Competences |> distinctSortCommaConcat
      FullName = fullName
      AssessmentForms = assessmentForms
      Lectures = impl.WorkHours.Lecture
      Seminars = impl.WorkHours.Seminar
      Consultations = impl.WorkHours.Consultation
      PracticalClasses = impl.WorkHours.Practical
      LaboratoryWorks = impl.WorkHours.Lab
      ControlWorks = impl.WorkHours.ControlWorks
      Colloquiums = impl.WorkHours.Colloquium
      CurrentAssessmentClassroom = impl.WorkHours.CurrentControl
      IntermediateAssessmentClassroom = impl.WorkHours.InterimAssessment
      UnderInstructorSupervision = impl.WorkHours.WithTeacherPresence
      InInstructorPresence = impl.WorkHours.WithTeacher
      UsingMaterials = impl.WorkHours.WithMethodologicalMaterials
      CurrentAssessmentIndependent = impl.WorkHours.CurrentControlIndependent
      IntermediateAssessmentIndependent = impl.WorkHours.MidtermAssessment
      InteractiveHours = impl.WorkHours.TotalIndependentWork
    }

let coursesToSemesterDtos (courses: Course list) : SemesterDto list =
    let allImpls = courses |> List.collect (fun c -> c.Implementations |> List.map (fun i -> (c, i)))
    let grouped = allImpls |> List.groupBy (fun (_, i) -> i.Semester) |> List.sortBy fst
    grouped |> List.map (fun (semNum, implPairs) ->
        let dtos = implPairs |> List.map (fun (c, i) -> toBaseDisciplineDto c i)
        { Number = semNum; BaseDisciplines = dtos })

type YearDto =
    { Number: int
      Semesters: SemesterDto list }

let semestersToYearsDto (semesters: SemesterDto list) : YearDto list =
    semesters
    |> List.sortBy _.Number
    |> List.groupBy (fun s -> (s.Number - 1) / 2 + 1)
    |> List.sortBy fst
    |> List.map (fun (yearNum, sems) ->
        { Number = yearNum
          Semesters = sems })

type PlanDto =
    { Competencies: CompetencyDto list
      Years: YearDto list }

let coursesToPlanDto (courses: Course list) : PlanDto =
    let competencies =
        courses
        |> List.collect (fun c -> c.Implementations |> List.collect _.Competences)
        |> List.distinct
        |> List.map (fun code -> { Code = code; Description = code })  // Описания отсутствуют, используем код

    let semesterDtos = coursesToSemesterDtos courses
    { Competencies = competencies
      Years = semestersToYearsDto semesterDtos }

let placeHeaderLine (ws: IXLWorksheet) (text: string) =
    let startRow = ws.LastRowUsed().RowBelow()
    startRow.FirstCell().SetValue text |> ignore

    ws.Range(startRow.RowNumber(), 1, startRow.RowNumber(), 20).Merge().Style
    |> _.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    |> _.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    |> _.Font.SetBold()
    |> ignore

let exportCompetencies (wb: XLWorkbook) dto =
    let competenciesSheet = wb.Worksheets.Add "Компетенции"
    competenciesSheet.Cell("A1").Value <- "Код компетенции"
    competenciesSheet.Cell("B1").Value <- "Наименование и (или) описание компетенции"
    
    // ИСПРАВЛЕНИЕ: Проверяем, есть ли данные
    if not (List.isEmpty dto.Competencies) then
        let insertedRange = competenciesSheet.Cell("A2").InsertData(dto.Competencies)
        insertedRange.Style.Alignment.WrapText <- true

    competenciesSheet.Column("A").Width <- 15
    competenciesSheet.Column("B").Width <- 78.67

let setupDisciplinesHeader (ws: IXLWorksheet) =
    let mergedCells =
        [ 1, "Код Блока"
          2, "Трудоёмкость,\nзачётных единиц"
          3, "Код компетенции"
          4, "Наименование дисциплины (модуля), практики,\nформы научно-исследовательской работы"
          5, "Виды текущего контроля успеваемости и (или) форма промежуточной аттестации"
          20, "Объём работы в активных и интерактивных формах, ак. ч." ]

    for col, text in mergedCells do
        ws.Cell(1, col).SetValue text |> ignore
        ws.Range(1, col, 2, col).Merge() |> ignore

    ws.Cell(1, 6).SetValue "Аудиторная работа обучающихся, часов" |> ignore
    ws.Range(1, 6, 1, 14).Merge() |> ignore
    ws.Cell(1, 15).SetValue "Самостоятельная работа, часов" |> ignore
    ws.Range(1, 15, 1, 19).Merge() |> ignore

    let workTypes =
        [ "Лекции"
          "Семинары"
          "Консультации"
          "Практические занятия"
          "Лабораторные работы"
          "Контрольные работы"
          "Коллоквиумы"
          "Текущий контроль"
          "Промежуточная аттестация"
          "Под руководством преподавателя"
          "В присутствии преподавателя"
          "В т.ч. с использованием учебно-методич. материалов"
          "Текущий контроль"
          "Промежуточная аттестация" ]

    for col, text in Seq.zip (Seq.initInfinite (fun n -> n + 6)) workTypes do
        ws.Cell(2, col).SetValue text |> ignore

    ws.Ranges("A1:C2, E1:E2, F2:S2, T1:T2").Style
    |> _.Alignment.SetTextRotation(90)
    |> ignore

    ws.Range("A1:T2").Style
    |> _.Alignment.SetWrapText(true)
    |> _.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    |> _.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    |> _.Font.SetBold()
    |> ignore

    ws.Column(1).Width <- 6.89
    ws.Column(2).Width <- 5.11
    ws.Column(3).Width <- 15
    ws.Column(4).Width <- 69.22
    ws.Column(5).Width <- 12.89
    ws.Columns("6:20").Width <- 4.44
    ws.Row(1).Height <- 23.85
    ws.Row(2).Height <- 156.75

let exportBaseDiscipline (ws: IXLWorksheet) (discipline: BaseDisciplineDto) =
    let startRow = ws.LastRowUsed().RowBelow()

    startRow
        .FirstCell()
        .SetValue(discipline.FgosCodeBlock)
        .CellRight()
        .SetValue(discipline.Workload)
        .CellRight()
        .SetValue(discipline.Competencies)
        .CellRight()
        .SetValue(discipline.FullName)
        .CellRight()
        .SetValue(discipline.AssessmentForms)
        .CellRight()
        .SetValue(discipline.Lectures)
        .CellRight()
        .SetValue(discipline.Seminars)
        .CellRight()
        .SetValue(discipline.Consultations)
        .CellRight()
        .SetValue(discipline.PracticalClasses)
        .CellRight()
        .SetValue(discipline.LaboratoryWorks)
        .CellRight()
        .SetValue(discipline.ControlWorks)
        .CellRight()
        .SetValue(discipline.Colloquiums)
        .CellRight()
        .SetValue(discipline.CurrentAssessmentClassroom)
        .CellRight()
        .SetValue(discipline.IntermediateAssessmentClassroom)
        .CellRight()
        .SetValue(discipline.UnderInstructorSupervision)
        .CellRight()
        .SetValue(discipline.InInstructorPresence)
        .CellRight()
        .SetValue(discipline.UsingMaterials)
        .CellRight()
        .SetValue(discipline.CurrentAssessmentIndependent)
        .CellRight()
        .SetValue(discipline.IntermediateAssessmentIndependent)
        .CellRight()
        .SetValue discipline.InteractiveHours
    |> ignore

    ws.Range(startRow.RowNumber(), 1, startRow.RowNumber(), 20).Style
    |> _.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    |> _.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    |> _.Alignment.SetWrapText(true)
    |> ignore

    ws.Range(startRow.RowNumber(), 4, startRow.RowNumber(), 5).Style
    |> _.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
    |> _.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    |> ignore

    startRow.AdjustToContents() |> ignore

let exportBaseDisciplines (ws: IXLWorksheet) (disciplines: BaseDisciplineDto list) =
    placeHeaderLine ws "Базовая часть периода обучения"  // TODO: Добавить разделы для Elective и Facultative

    if List.length disciplines = 0 then
        placeHeaderLine ws "Не предусмотрено"
    else
        disciplines |> List.iter (exportBaseDiscipline ws)

let exportSemester (ws: IXLWorksheet) (semester: SemesterDto) =
    placeHeaderLine ws $"С%02d{semester.Number}. Семестр %d{semester.Number}"
    exportBaseDisciplines ws semester.BaseDisciplines

let exportYear (ws: IXLWorksheet) (year: YearDto) =
    placeHeaderLine ws $"{year.Number} год обучения"

    year.Semesters |> List.iter (exportSemester ws)

let exportDisciplines (wb: XLWorkbook) dto =
    let planSheet = wb.Worksheets.Add "План"

    setupDisciplinesHeader planSheet
    dto.Years |> List.iter (exportYear planSheet)

let exportToExcel (filePath: string) (courses: Course list) : unit =
    try
        printfn "Начинаем экспорт %d курсов в Excel..." courses.Length
        
        if List.isEmpty courses then
            printfn "Нет данных для экспорта"
        
        let dto = coursesToPlanDto courses
        printfn "Создан DTO: %d компетенций, %d лет обучения" dto.Competencies.Length dto.Years.Length
        
        let wb = new XLWorkbook()
        exportCompetencies wb dto
        exportDisciplines wb dto
        
        wb.SaveAs filePath
        printfn "✓ Excel файл создан: %s" filePath
        
    with ex ->
        printfn "✗ Ошибка при экспорте в Excel: %s" ex.Message
        printfn "StackTrace: %s" ex.StackTrace
        raise ex