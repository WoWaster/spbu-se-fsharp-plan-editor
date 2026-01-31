module PlanEditor.ExcelExport

open BySemesterModel
open ClosedXML.Excel


// Helpers
let placeHeaderLine (ws: IXLWorksheet) (text: string) =
    let startRow = ws.LastRowUsed().RowBelow()
    startRow.FirstCell().SetValue text |> ignore

    ws.Range(startRow.RowNumber(), 1, startRow.RowNumber(), 20).Merge().Style
    |> _.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    |> _.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    |> _.Font.SetBold()
    |> ignore

let distinctSortMapCommaConcat f (items: 'a list) : string =
    items |> List.distinct |> List.sort |> List.map f |> String.concat ", "

let distinctSortCommaConcat items = distinctSortMapCommaConcat id items

let fgosBlockCodeToString (code: FgosBlockCode) : string =
    match code with
    | Disciplines -> "Блок.1.дисц"
    | PracticalTraining -> "Блок.2.прки"
    | Gia -> "Блок.3.гиа"

let assessmentFormToString (form: AssessmentForm) : string =
    match form with
    | Exam -> "экзамен"
    | Credit -> "зачёт"
    | AttestationTest -> "аттестационное испытание"


// Competencies
let exportCompetencies (wb: XLWorkbook) plan =
    let competenciesSheet = wb.Worksheets.Add "Компетенции"
    competenciesSheet.Cell("A1").Value <- "Код компетенции"
    competenciesSheet.Cell("B1").Value <- "Наименование и (или) описание компетенции"

    let insertedRange =
        competenciesSheet |> _.Cell("A2") |> _.InsertData(plan.Competencies)

    competenciesSheet.Column("A").Width <- 15
    competenciesSheet.Column("B").Width <- 78.67
    insertedRange.Style.Alignment.WrapText <- true

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

    ws.SheetView.FreezeRows 2


// Disciplines

let exportDiscipline (ws: IXLWorksheet) row (discipline: Discipline) =
    let fullName =
        $"[%06d{discipline.Number}] %s{discipline.Name}\n%s{discipline.EnglishName}"

    let assessmentFormsStr =
        discipline.AssessmentForms |> distinctSortMapCommaConcat assessmentFormToString

    ws
        .Cell(row, 4)
        .SetValue(fullName)
        .CellRight()
        .SetValue(assessmentFormsStr)
        .CellRight()
        .SetValue(discipline.ClassroomWork.Lectures)
        .CellRight()
        .SetValue(discipline.ClassroomWork.Seminars)
        .CellRight()
        .SetValue(discipline.ClassroomWork.Consultations)
        .CellRight()
        .SetValue(discipline.ClassroomWork.PracticalClasses)
        .CellRight()
        .SetValue(discipline.ClassroomWork.LaboratoryWorks)
        .CellRight()
        .SetValue(discipline.ClassroomWork.ControlWorks)
        .CellRight()
        .SetValue(discipline.ClassroomWork.Colloquiums)
        .CellRight()
        .SetValue(discipline.ClassroomWork.CurrentAssessment)
        .CellRight()
        .SetValue(discipline.ClassroomWork.IntermediateAssessment)
        .CellRight()
        .SetValue(discipline.IndependentWork.UnderInstructorSupervision)
        .CellRight()
        .SetValue(discipline.IndependentWork.InInstructorPresence)
        .CellRight()
        .SetValue(discipline.IndependentWork.UsingMaterials)
        .CellRight()
        .SetValue(discipline.IndependentWork.CurrentAssessment)
        .CellRight()
        .SetValue(discipline.IndependentWork.IntermediateAssessment)
        .CellRight()
        .SetValue
        discipline.InteractiveHours
    |> ignore

let exportSimpleBlock (ws: IXLWorksheet) block =
    let rowsCount = block.Disciplines |> List.length
    let startRow = ws.LastRowUsed().RowBelow().FirstCell().Address |> _.RowNumber

    ws.Cell(startRow, 1).Value <- fgosBlockCodeToString block.FgosBlockCode
    ws.Range(startRow, 1, startRow + rowsCount - 1, 1).Merge() |> ignore
    ws.Cell(startRow, 2).Value <- block.Workload
    ws.Range(startRow, 2, startRow + rowsCount - 1, 2).Merge() |> ignore
    let competenciesString = block.Competencies |> distinctSortCommaConcat
    ws.Cell(startRow, 3).Value <- competenciesString
    ws.Range(startRow, 3, startRow + rowsCount - 1, 3).Merge() |> ignore

    for i, discipline in List.indexed block.Disciplines do
        exportDiscipline ws (startRow + i) discipline

    ws.Range(startRow, 1, startRow + rowsCount, 20).Style
    |> _.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
    |> _.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
    |> _.Alignment.SetWrapText(true)
    |> ignore

    ws.Range(startRow, 4, startRow + rowsCount, 5).Style
    |> _.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
    |> ignore

let exportComplexBlock ws (block: ComplexBlock) =
    placeHeaderLine ws $"Блок дисциплин {block.Name}"

    for t in block.Tracks do
        placeHeaderLine ws t.Key
        t.Value |> List.iter (exportSimpleBlock ws)

let exportSemester (ws: IXLWorksheet) sem =
    if sem.Number % 2 = 1 then
        placeHeaderLine ws $"{(sem.Number - 1) / 2 + 1} год обучения"

    placeHeaderLine ws $"С%02d{sem.Number}. Семестр %d{sem.Number}"

    placeHeaderLine ws "Базовая часть периода обучения"

    if
        List.length sem.BasicBlocks.SimpleBlocks
        + List.length sem.BasicBlocks.ComplexBlocks = 0
    then
        placeHeaderLine ws "Не предусмотрено"
    else
        sem.BasicBlocks.SimpleBlocks |> List.iter (exportSimpleBlock ws)

        if List.length sem.BasicBlocks.ComplexBlocks <> 0 then
            placeHeaderLine ws $"Блок(и) дисциплин"
            sem.BasicBlocks.ComplexBlocks |> List.iter (exportComplexBlock ws)


    placeHeaderLine ws "Вариативная часть периода обучения"

    if
        List.length sem.ElectiveBlocks.SimpleBlocks
        + List.length sem.ElectiveBlocks.ComplexBlocks = 0
    then
        placeHeaderLine ws "Не предусмотрено"
    else
        sem.ElectiveBlocks.SimpleBlocks |> List.iter (exportSimpleBlock ws)

        if List.length sem.ElectiveBlocks.ComplexBlocks <> 0 then
            placeHeaderLine ws $"Блок(и) дисциплин"
            sem.ElectiveBlocks.ComplexBlocks |> List.iter (exportComplexBlock ws)

    ()

let exportDisciplines (wb: XLWorkbook) plan =
    let planSheet = wb.Worksheets.Add "План"

    setupDisciplinesHeader planSheet
    plan.Semesters |> List.sortBy _.Number |> List.iter (exportSemester planSheet)

let exportToExcel (filePath: string) (plan: Plan) : unit =
    let wb = new XLWorkbook()
    exportCompetencies wb plan
    exportDisciplines wb plan
    wb.SaveAs filePath
