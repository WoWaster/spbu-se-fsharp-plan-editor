module PlanEditor.BySemesterDSL

open BySemesterModel

type ClassroomWorkBuilder() =
    member inline _.Yield _ = ClassroomWork.Empty

    [<CustomOperation("lectures")>]
    member inline _.SetLectures(state, n) = { state with Lectures = n }

    [<CustomOperation("seminars")>]
    member inline _.SetSeminars(state, n) = { state with Seminars = n }

    [<CustomOperation("consultations")>]
    member inline _.SetConsultations(state, n) = { state with Consultations = n }

    [<CustomOperation("practicalClasses")>]
    member inline _.SetPracticalClasses(state, n) = { state with PracticalClasses = n }

    [<CustomOperation("laboratoryWorks")>]
    member inline _.SetLaboratoryWorks(state, n) = { state with LaboratoryWorks = n }

    [<CustomOperation("controlWorks")>]
    member inline _.SetControlWorks(state, n) = { state with ControlWorks = n }

    [<CustomOperation("colloquiums")>]
    member inline _.SetColloquiums(state, n) = { state with Colloquiums = n }

    [<CustomOperation("currentAssessment")>]
    member inline _.SetCurrentAssessment(state, n) =
        { state with
            ClassroomWork.CurrentAssessment = n }

    [<CustomOperation("intermediateAssessment")>]
    member inline _.SetIntermediateAssessment(state, n) =
        { state with
            ClassroomWork.IntermediateAssessment = n }

let classroomWork = ClassroomWorkBuilder()

type IndependentWorkBuilder() =
    member inline _.Yield _ = IndependentWork.Empty

    [<CustomOperation("underInstructorSupervision")>]
    member inline _.SetUnderInstructorSupervision(state, n) =
        { state with
            UnderInstructorSupervision = n }

    [<CustomOperation("inInstructorPresence")>]
    member inline _.SetInInstructorPresence(state, n) = { state with InInstructorPresence = n }

    [<CustomOperation("usingMaterials")>]
    member inline _.SetUsingMaterials(state, n) = { state with UsingMaterials = n }

    [<CustomOperation("currentAssessment")>]
    member inline _.SetCurrentAssessment(state, n) =
        { state with
            IndependentWork.CurrentAssessment = n }

    [<CustomOperation("intermediateAssessment")>]
    member inline _.SetIntermediateAssessment(state, n) =
        { state with
            IndependentWork.IntermediateAssessment = n }

let independentWork = IndependentWorkBuilder()

type DisciplineBuilder() =
    member inline _.Yield _ =
        { Number = 999999
          Name = ""
          EnglishName = ""
          Realization = ""
          Trajectory = ""
          AssessmentForms = []
          ClassroomWork = ClassroomWork.Empty
          IndependentWork = IndependentWork.Empty
          InteractiveHours = 0 }

    member inline _.Run state = state

    [<CustomOperation("number")>]
    member inline _.SetNumber(state, n) = { state with Discipline.Number = n }

    [<CustomOperation("name")>]
    member inline _.SetName(state, s) = { state with Discipline.Name = s }

    [<CustomOperation("englishName")>]
    member inline _.SetEnglishName(state, s) =
        { state with
            Discipline.EnglishName = s }

    [<CustomOperation("realization")>]
    member inline _.SetRealization(state, s) = { state with Realization = s }

    [<CustomOperation("trajectory")>]
    member inline _.SetTrajectory(state, s) = { state with Trajectory = s }

    [<CustomOperation("assessmentForms")>]
    member inline _.SetAssessmentForms(state, forms) = { state with AssessmentForms = forms }

    [<CustomOperation("classroomWork")>]
    member inline _.SetClassroomWork(state, work) = { state with ClassroomWork = work }

    [<CustomOperation("independentWork")>]
    member inline _.SetIndependentWork(state, work) = { state with IndependentWork = work }

    [<CustomOperation("interactiveHours")>]
    member inline _.SetInteractiveHours(state, n) = { state with InteractiveHours = n }

let discipline = DisciplineBuilder()

type SimpleBlockBuilder() =
    member inline _.Yield _ =
        { FgosBlockCode = Disciplines
          Workload = 0
          Competencies = []
          Disciplines = [] }

    member inline _.Run state = state

    [<CustomOperation("fgosBlockCode")>]
    member inline _.SetFgosBlockCode(state, code) = { state with FgosBlockCode = code }

    [<CustomOperation("workload")>]
    member inline _.SetWorkload(state, n) = { state with Workload = n }

    [<CustomOperation("competencies")>]
    member inline _.SetCompetencies(state, comps) =
        { state with
            SimpleBlock.Competencies = comps }

    [<CustomOperation("disciplines")>]
    member inline _.SetDisciplines(state, discs) = { state with Disciplines = discs }


let simpleBlock = SimpleBlockBuilder()

type ComplexBlockBuilder() =
    member _.Yield _ = { Name = ""; Tracks = Map.empty }
    member inline _.Run state = state

    [<CustomOperation("name")>]
    member inline _.SetName(state, s) = { state with ComplexBlock.Name = s }

    [<CustomOperation("tracks")>]
    member inline _.SetTracks(state, tracks) = { state with Tracks = tracks }

let complexBlock = ComplexBlockBuilder()

type BlocksBuilder() =
    member _.Yield _ =
        { SimpleBlocks = []
          ComplexBlocks = [] }

    member inline _.Run state = state

    [<CustomOperation("simpleBlocks")>]
    member inline _.SetSimpleBlocks(state, blocks) = { state with SimpleBlocks = blocks }

    [<CustomOperation("complexBlocks")>]
    member inline _.SetComplexBlocks(state, blocks) = { state with ComplexBlocks = blocks }

let blocks = BlocksBuilder()

type SemesterBuilder() =
    member _.Yield _ =
        { Semester.Number = 0
          BasicBlocks =
            { SimpleBlocks = []
              ComplexBlocks = [] }
          ElectiveBlocks =
            { SimpleBlocks = []
              ComplexBlocks = [] } }

    member inline _.Run state = state

    [<CustomOperation("number")>]
    member inline _.SetNumber(state, n) = { state with Semester.Number = n }

    [<CustomOperation("basicBlocks")>]
    member inline _.SetBasicBlocks(state, blocks) = { state with BasicBlocks = blocks }

    [<CustomOperation("electiveBlocks")>]
    member inline _.SetElectiveBlocks(state, blocks) = { state with ElectiveBlocks = blocks }

let semester = SemesterBuilder()

type PlanBuilder() =
    member _.Yield(_) =
        { Plan.Name = ""
          EnglishName = ""
          StudyLevel = Bachelor
          Specialty = ""
          LanguagesOfInstruction = [ Russian; English ]
          YearOfAdmission = 26
          Code = 9999
          Competencies = Map.empty
          Semesters = [] }

    [<CustomOperation("name")>]
    member inline _.SetName(state, s) = { state with Plan.Name = s }

    [<CustomOperation("englishName")>]
    member inline _.SetEnglishName(state, s) = { state with Plan.EnglishName = s }

    [<CustomOperation("studyLevel")>]
    member inline _.SetStudyLevel(state, level) = { state with StudyLevel = level }

    [<CustomOperation("specialty")>]
    member inline _.SetSpecialty(state, s) = { state with Specialty = s }

    [<CustomOperation("languagesOfInstruction")>]
    member inline _.SetLanguagesOfInstruction(state, langs) =
        { state with
            LanguagesOfInstruction = langs }

    [<CustomOperation("yearOfAdmission")>]
    member inline _.SetYearOfAdmission(state, year) = { state with YearOfAdmission = year }

    [<CustomOperation("code")>]
    member inline _.SetCode(state, n) = { state with Code = n }

    [<CustomOperation("competencies")>]
    member inline _.SetCompetencies(state, comps) =
        { state with Plan.Competencies = comps }

    [<CustomOperation("semesters")>]
    member inline _.SetSemesters(state, sems) = { state with Semesters = sems }

let plan = PlanBuilder()
