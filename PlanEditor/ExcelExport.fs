module PlanEditor.ExcelExport

open PlanEditor.BySemesterModel
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

let assessmentFormToString (form: AssessmentForm) : string =
    match form with
    | Exam -> "экзамен"
    | Credit -> "зачёт"
    | AttestationTest -> "аттестационное испытание"

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

let semesterToSemesterDto (semester: Semester) : SemesterDto =
    let baseDisciplineToBaseDisciplineDto (bd: BaseDiscipline) : BaseDisciplineDto =
        { FgosCodeBlock = fgosBlockCodeToString bd.Info.FgosBlockCode
          Workload = bd.Info.Workload
          Competencies = bd.Info.Competencies |> distinctSortCommaConcat
          FullName = $"[%06d{bd.Discipline.Number}] %s{bd.Discipline.Name}\n%s{bd.Discipline.EnglishName}"
          AssessmentForms = bd.Info.AssessmentForms |> distinctSortMapCommaConcat assessmentFormToString
          Lectures = bd.Discipline.ClassroomWork.Lectures
          Seminars = bd.Discipline.ClassroomWork.Seminars
          Consultations = bd.Discipline.ClassroomWork.Consultations
          PracticalClasses = bd.Discipline.ClassroomWork.PracticalClasses
          LaboratoryWorks = bd.Discipline.ClassroomWork.LaboratoryWorks
          ControlWorks = bd.Discipline.ClassroomWork.ControlWorks
          Colloquiums = bd.Discipline.ClassroomWork.Lectures
          CurrentAssessmentClassroom = bd.Discipline.ClassroomWork.CurrentAssessment
          IntermediateAssessmentClassroom = bd.Discipline.ClassroomWork.IntermediateAssessment
          UnderInstructorSupervision = bd.Discipline.IndependentWork.UnderInstructorSupervision
          InInstructorPresence = bd.Discipline.IndependentWork.InInstructorPresence
          UsingMaterials = bd.Discipline.IndependentWork.UsingMaterials
          CurrentAssessmentIndependent = bd.Discipline.IndependentWork.CurrentAssessment
          IntermediateAssessmentIndependent = bd.Discipline.IndependentWork.IntermediateAssessment
          InteractiveHours = bd.Discipline.InteractiveHours }

    let baseDisciplines =
        semester.Disciplines
        |> List.choose (fun d ->
            match d with
            | BaseDiscipline d -> Some d
            | _ -> None)
        |> List.map baseDisciplineToBaseDisciplineDto

    { Number = semester.Number
      BaseDisciplines = baseDisciplines }

type YearDto =
    { Number: int
      Semesters: SemesterDto list }

let semestersToYearsDto (semesters: Semester list) : YearDto list =
    semesters
    |> List.sortBy _.Number
    |> List.groupBy (fun s -> (s.Number - 1) / 2 + 1)
    |> List.sortBy fst
    |> List.map (fun (yearNum, sems) ->
        { Number = yearNum
          Semesters = sems |> List.map semesterToSemesterDto })

type PlanDto =
    { Competencies: CompetencyDto list
      Years: YearDto list }

let planToPlanDto (plan: Plan) : PlanDto =
    { Competencies =
        plan.Competencies
        |> Map.toList
        |> List.map (fun (code, desc) -> { Code = code; Description = desc })
      Years = plan.Semesters |> semestersToYearsDto }

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
    let insertedRange = competenciesSheet.Cell("A2").InsertData(dto.Competencies)

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
        .SetValue
        discipline.InteractiveHours
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
    placeHeaderLine ws "Базовая часть периода обучения"

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


let exportToExcel (filePath: string) (dto: PlanDto) : unit =
    let wb = new XLWorkbook()
    exportCompetencies wb dto
    exportDisciplines wb dto
    wb.SaveAs filePath
