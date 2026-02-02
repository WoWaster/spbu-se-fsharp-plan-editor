module PlanEditor.BySemesterBuilders

open BySemesterModel
open DSLCommon

// Classroom Work
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

// Independent Work
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

// Discipline
// TODO: Is required?
[<RequireQualifiedAccess>]
type DisciplineProperty =
    | Number of int
    | Name of string
    | EnglishName of string
    | Realization of string
    | Trajectory of string
    | AssessmentForms of AssessmentForm list
    | ClassroomWork of ClassroomWork
    | IndependentWork of IndependentWork
    | InteractiveHours of int

    static member Folder (discipline: Discipline) (prop: DisciplineProperty) =
        match prop with
        | Number n -> { discipline with Number = n }
        | Name n -> { discipline with Name = n }
        | EnglishName n -> { discipline with EnglishName = n }
        | Realization r -> { discipline with Realization = r }
        | Trajectory t -> { discipline with Trajectory = t }
        | AssessmentForms af -> { discipline with AssessmentForms = af }
        | ClassroomWork cw -> { discipline with ClassroomWork = cw }
        | IndependentWork iw -> { discipline with IndependentWork = iw }
        | InteractiveHours ih ->
            { discipline with
                InteractiveHours = ih }

type DisciplineBuilder() =
    inherit DSLBuilder<Discipline, DisciplineProperty>(Discipline.Empty, DisciplineProperty.Folder)

    member inline _.Yield(iw: IndependentWork) =
        [ DisciplineProperty.IndependentWork iw ]

    member inline _.Yield(cw: ClassroomWork) = [ DisciplineProperty.ClassroomWork cw ]

    [<CustomOperation("number")>]
    member inline this.SetNumber(props: DisciplineProperty list, n) =
        this.Combine(DisciplineProperty.Number n, props)

    [<CustomOperation("name")>]
    member inline this.SetName(props: DisciplineProperty list, name) =
        this.Combine(DisciplineProperty.Name name, props)

    [<CustomOperation("englishName")>]
    member inline this.SetEnglishName(props: DisciplineProperty list, name) =
        this.Combine(DisciplineProperty.EnglishName name, props)

    [<CustomOperation("realization")>]
    member inline this.SetRealization(props: DisciplineProperty list, realization) =
        this.Combine(DisciplineProperty.Realization realization, props)

    [<CustomOperation("trajectory")>]
    member inline this.SetTrajectory(props: DisciplineProperty list, trajectory) =
        this.Combine(DisciplineProperty.Trajectory trajectory, props)

    [<CustomOperation("assessmentForms")>]
    member inline this.SetAssessmentForms(props: DisciplineProperty list, forms) =
        this.Combine(DisciplineProperty.AssessmentForms forms, props)

    [<CustomOperation("interactiveHours")>]
    member inline this.SetInteractiveHours(props: DisciplineProperty list, hours) =
        this.Combine(DisciplineProperty.InteractiveHours hours, props)

// Simple Block
// TODO: Is required?
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

// Complex Block
// TODO: Is required?
type ComplexBlockBuilder() =
    member _.Yield _ = { Name = ""; Tracks = Map.empty }
    member inline _.Run state = state

    [<CustomOperation("name")>]
    member inline _.SetName(state, s) = { state with ComplexBlock.Name = s }

    [<CustomOperation("tracks")>]
    member inline _.SetTracks(state, tracks) = { state with Tracks = tracks }

// Blocks
// TODO: Is required?
type BlocksBuilder() =
    member _.Yield _ =
        { SimpleBlocks = []
          ComplexBlocks = [] }

    member inline _.Run state = state

    [<CustomOperation("simpleBlocks")>]
    member inline _.SetSimpleBlocks(state, blocks) = { state with SimpleBlocks = blocks }

    [<CustomOperation("complexBlocks")>]
    member inline _.SetComplexBlocks(state, blocks) = { state with ComplexBlocks = blocks }

// Semester
type SemesterBuilder() =
    member _.Yield _ =
        { Semester.Number = 0
          BasicPart =
            { SimpleBlocks = []
              ComplexBlocks = [] }
          VariablePart =
            { SimpleBlocks = []
              ComplexBlocks = [] } }

    member inline _.Run state = state

    [<CustomOperation("number")>]
    member inline _.SetNumber(state, n) = { state with Semester.Number = n }

    [<CustomOperation("basicBlocks")>]
    member inline _.SetBasicBlocks(state, blocks) = { state with BasicPart = blocks }

    [<CustomOperation("electiveBlocks")>]
    member inline _.SetElectiveBlocks(state, blocks) = { state with VariablePart = blocks }

// Plan
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

// Simple Discipline
[<RequireQualifiedAccess>]
type SimpleDisciplineProperty =
    | FgosBlockCode of FgosBlockCode
    | Workload of int
    | Competencies of string list
    | Number of int
    | Name of string
    | EnglishName of string
    | Realization of string
    | Trajectory of string
    | AssessmentForms of AssessmentForm list
    | ClassroomWork of ClassroomWork
    | IndependentWork of IndependentWork
    | InteractiveHours of int

    static member Folder (block: SimpleBlock) (prop: SimpleDisciplineProperty) =
        match prop with
        | FgosBlockCode code -> { block with FgosBlockCode = code }
        | Workload n -> { block with Workload = n }
        | Competencies cs -> { block with Competencies = cs }
        | Number n ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          Number = n } ] }
        | Name n ->
            { block with
                Disciplines = [ { block.Disciplines.Head with Name = n } ] }
        | EnglishName n ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          EnglishName = n } ] }
        | Realization r ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          Realization = r } ] }
        | Trajectory t ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          Trajectory = t } ] }
        | AssessmentForms af ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          AssessmentForms = af } ] }
        | ClassroomWork cw ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          ClassroomWork = cw } ] }
        | IndependentWork iw ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          IndependentWork = iw } ] }
        | InteractiveHours ih ->
            { block with
                Disciplines =
                    [ { block.Disciplines.Head with
                          InteractiveHours = ih } ] }

let simpleDisciplineEmpty =
    { FgosBlockCode = Disciplines
      Workload = 0
      Competencies = []
      Disciplines = [ Discipline.Empty ] }

type SimpleDisciplineBuilder() =
    inherit DSLBuilder<SimpleBlock, SimpleDisciplineProperty>(simpleDisciplineEmpty, SimpleDisciplineProperty.Folder)

    member inline _.Yield(iw: IndependentWork) =
        [ SimpleDisciplineProperty.IndependentWork iw ]

    member inline _.Yield(cw: ClassroomWork) =
        [ SimpleDisciplineProperty.ClassroomWork cw ]

    [<CustomOperation("fgosBlockCode")>]
    member inline this.SetFgosBlockCode(props: SimpleDisciplineProperty list, code) =
        this.Combine(SimpleDisciplineProperty.FgosBlockCode code, props)


    [<CustomOperation("workload")>]
    member inline this.SetWorkload(props: SimpleDisciplineProperty list, n) =
        this.Combine(SimpleDisciplineProperty.Workload n, props)

    [<CustomOperation("competencies")>]
    member inline this.SetCompetencies(props: SimpleDisciplineProperty list, cs) =
        this.Combine(SimpleDisciplineProperty.Competencies cs, props)

    [<CustomOperation("number")>]
    member inline this.SetNumber(props: SimpleDisciplineProperty list, n) =
        this.Combine(SimpleDisciplineProperty.Number n, props)

    [<CustomOperation("name")>]
    member inline this.SetName(props: SimpleDisciplineProperty list, name) =
        this.Combine(SimpleDisciplineProperty.Name name, props)

    [<CustomOperation("englishName")>]
    member inline this.SetEnglishName(props: SimpleDisciplineProperty list, name) =
        this.Combine(SimpleDisciplineProperty.EnglishName name, props)

    [<CustomOperation("realization")>]
    member inline this.SetRealization(props: SimpleDisciplineProperty list, realization) =
        this.Combine(SimpleDisciplineProperty.Realization realization, props)

    [<CustomOperation("trajectory")>]
    member inline this.SetTrajectory(props: SimpleDisciplineProperty list, trajectory) =
        this.Combine(SimpleDisciplineProperty.Trajectory trajectory, props)

    [<CustomOperation("assessmentForms")>]
    member inline this.SetAssessmentForms(props: SimpleDisciplineProperty list, forms) =
        this.Combine(SimpleDisciplineProperty.AssessmentForms forms, props)

    [<CustomOperation("interactiveHours")>]
    member inline this.SetInteractiveHours(props: SimpleDisciplineProperty list, hours) =
        this.Combine(SimpleDisciplineProperty.InteractiveHours hours, props)
