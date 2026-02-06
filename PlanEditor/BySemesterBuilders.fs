module PlanEditor.BySemesterBuilders

open BySemesterModel
open DSLCommon

// Classroom Work
[<RequireQualifiedAccess>]
type ClassroomWorkProperty =
    | Lectures of int
    | Seminars of int
    | Consultations of int
    | PracticalClasses of int
    | LaboratoryWorks of int
    | ControlWorks of int
    | Colloquiums of int
    | CurrentAssessment of int
    | IntermediateAssessment of int

    static member Folder (classroomWork: ClassroomWork) (prop: ClassroomWorkProperty) =
        match prop with
        | Lectures n -> { classroomWork with Lectures = n }
        | Seminars n -> { classroomWork with Seminars = n }
        | Consultations n -> { classroomWork with Consultations = n }
        | PracticalClasses n ->
            { classroomWork with
                PracticalClasses = n }
        | LaboratoryWorks n ->
            { classroomWork with
                LaboratoryWorks = n }
        | ControlWorks n -> { classroomWork with ControlWorks = n }
        | Colloquiums n -> { classroomWork with Colloquiums = n }
        | CurrentAssessment n ->
            { classroomWork with
                CurrentAssessment = n }
        | IntermediateAssessment n ->
            { classroomWork with
                IntermediateAssessment = n }

type ClassroomWorkBuilder() =
    inherit DSLBuilder<ClassroomWork, ClassroomWorkProperty>(ClassroomWork.Empty, ClassroomWorkProperty.Folder)

    [<CustomOperation("lectures")>]
    member inline this.SetLectures(props, n) =
        this.Combine(ClassroomWorkProperty.Lectures n, props)

    [<CustomOperation("seminars")>]
    member inline this.SetSeminars(props, n) =
        this.Combine(ClassroomWorkProperty.Seminars n, props)

    [<CustomOperation("consultations")>]
    member inline this.SetConsultations(props, n) =
        this.Combine(ClassroomWorkProperty.Consultations n, props)

    [<CustomOperation("practicalClasses")>]
    member inline this.SetPracticalClasses(props, n) =
        this.Combine(ClassroomWorkProperty.PracticalClasses n, props)

    [<CustomOperation("laboratoryWorks")>]
    member inline this.SetLaboratoryWorks(props, n) =
        this.Combine(ClassroomWorkProperty.LaboratoryWorks n, props)

    [<CustomOperation("controlWorks")>]
    member inline this.SetControlWorks(props, n) =
        this.Combine(ClassroomWorkProperty.ControlWorks n, props)

    [<CustomOperation("colloquiums")>]
    member inline this.SetColloquiums(props, n) =
        this.Combine(ClassroomWorkProperty.Colloquiums n, props)

    [<CustomOperation("currentAssessment")>]
    member inline this.SetCurrentAssessment(props, n) =
        this.Combine(ClassroomWorkProperty.CurrentAssessment n, props)

    [<CustomOperation("intermediateAssessment")>]
    member inline this.SetIntermediateAssessment(props, n) =
        this.Combine(ClassroomWorkProperty.IntermediateAssessment n, props)

// Independent Work
[<RequireQualifiedAccess>]
type IndependentWorkProperty =
    | UnderInstructorSupervision of int
    | InInstructorPresence of int
    | UsingMaterials of int
    | CurrentAssessment of int
    | IntermediateAssessment of int

    static member Folder (independentWork: IndependentWork) (prop: IndependentWorkProperty) =
        match prop with
        | UnderInstructorSupervision n ->
            { independentWork with
                UnderInstructorSupervision = n }
        | InInstructorPresence n ->
            { independentWork with
                InInstructorPresence = n }
        | UsingMaterials n ->
            { independentWork with
                UsingMaterials = n }
        | CurrentAssessment n ->
            { independentWork with
                CurrentAssessment = n }
        | IntermediateAssessment n ->
            { independentWork with
                IntermediateAssessment = n }

type IndependentWorkBuilder() =
    inherit DSLBuilder<IndependentWork, IndependentWorkProperty>(IndependentWork.Empty, IndependentWorkProperty.Folder)

    [<CustomOperation("underInstructorSupervision")>]
    member inline this.SetUnderInstructorSupervision(props, n) =
        this.Combine(IndependentWorkProperty.UnderInstructorSupervision n, props)

    [<CustomOperation("inInstructorPresence")>]
    member inline this.SetInInstructorPresence(props, n) =
        this.Combine(IndependentWorkProperty.InInstructorPresence n, props)

    [<CustomOperation("usingMaterials")>]
    member inline this.SetUsingMaterials(props, n) =
        this.Combine(IndependentWorkProperty.UsingMaterials n, props)

    [<CustomOperation("currentAssessment")>]
    member inline this.SetCurrentAssessment(props, n) =
        this.Combine(IndependentWorkProperty.CurrentAssessment n, props)

    [<CustomOperation("intermediateAssessment")>]
    member inline this.SetIntermediateAssessment(props, n) =
        this.Combine(IndependentWorkProperty.IntermediateAssessment n, props)

// Discipline in Elective Block
[<RequireQualifiedAccess>]
type DisciplineInBlockProperty =
    | Number of int
    | Name of string
    | EnglishName of string
    | Realization of string
    | Trajectory of string
    | AssessmentForms of AssessmentForm list
    | ClassroomWork of ClassroomWork
    | IndependentWork of IndependentWork
    | InteractiveHours of int

    static member Folder (discipline: Discipline) (prop: DisciplineInBlockProperty) =
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

type DisciplineInBlockBuilder() =
    inherit DSLBuilder<Discipline, DisciplineInBlockProperty>(Discipline.Empty, DisciplineInBlockProperty.Folder)

    member inline _.Yield(iw: IndependentWork) =
        [ DisciplineInBlockProperty.IndependentWork iw ]

    member inline _.Yield(cw: ClassroomWork) =
        [ DisciplineInBlockProperty.ClassroomWork cw ]

    [<CustomOperation("number")>]
    member inline this.SetNumber(props: DisciplineInBlockProperty list, n) =
        this.Combine(DisciplineInBlockProperty.Number n, props)

    [<CustomOperation("name")>]
    member inline this.SetName(props: DisciplineInBlockProperty list, name) =
        this.Combine(DisciplineInBlockProperty.Name name, props)

    [<CustomOperation("englishName")>]
    member inline this.SetEnglishName(props: DisciplineInBlockProperty list, name) =
        this.Combine(DisciplineInBlockProperty.EnglishName name, props)

    [<CustomOperation("realization")>]
    member inline this.SetRealization(props: DisciplineInBlockProperty list, realization) =
        this.Combine(DisciplineInBlockProperty.Realization realization, props)

    [<CustomOperation("trajectory")>]
    member inline this.SetTrajectory(props: DisciplineInBlockProperty list, trajectory) =
        this.Combine(DisciplineInBlockProperty.Trajectory trajectory, props)

    [<CustomOperation("assessmentForms")>]
    member inline this.SetAssessmentForms(props: DisciplineInBlockProperty list, forms) =
        this.Combine(DisciplineInBlockProperty.AssessmentForms forms, props)

    [<CustomOperation("interactiveHours")>]
    member inline this.SetInteractiveHours(props: DisciplineInBlockProperty list, hours) =
        this.Combine(DisciplineInBlockProperty.InteractiveHours hours, props)

// Elective Block
[<RequireQualifiedAccess>]
type ElectiveBlockProperty =
    | FgosBlockCode of FgosBlockCode
    | Workload of int
    | Competencies of string list
    | Disciplines of Discipline list

    static member Folder (simpleBlock: SimpleBlock) (prop: ElectiveBlockProperty) =
        match prop with
        | FgosBlockCode code ->
            { simpleBlock with
                FgosBlockCode = code }
        | Workload n -> { simpleBlock with Workload = n }
        | Competencies comps ->
            { simpleBlock with
                Competencies = comps }
        | Disciplines disciplines ->
            { simpleBlock with
                Disciplines = disciplines }

type ElectiveBlockBuilder() =
    inherit DSLBuilder<SimpleBlock, ElectiveBlockProperty>(SimpleBlock.Empty, ElectiveBlockProperty.Folder)

    [<CustomOperation("fgosBlockCode")>]
    member inline this.SetFgosBlockCode(props, code) =
        this.Combine(ElectiveBlockProperty.FgosBlockCode code, props)

    [<CustomOperation("workload")>]
    member inline this.SetWorkload(props, n) =
        this.Combine(ElectiveBlockProperty.Workload n, props)

    [<CustomOperation("competencies")>]
    member inline this.SetCompetencies(props, comps) =
        this.Combine(ElectiveBlockProperty.Competencies comps, props)

    [<CustomOperation("disciplines")>]
    member inline this.SetDisciplines(props, disciplines) =
        this.Combine(ElectiveBlockProperty.Disciplines disciplines, props)

// Basic Part
[<RequireQualifiedAccess>]
type BasicPartProperty =
    | SimpleBlock of SimpleBlock
    | ComplexBlock of ComplexBlock

    static member Folder (bp: BasicPart) (prop: BasicPartProperty) =
        let part = bp.Value

        (match prop with
         | SimpleBlock sb ->
             { part with
                 SimpleBlocks = sb :: part.SimpleBlocks }
         | ComplexBlock cb ->
             { part with
                 ComplexBlocks = cb :: part.ComplexBlocks })
        |> BasicPart

type BasicPartBuilder() =
    inherit DSLBuilder<BasicPart, BasicPartProperty>(BasicPart.Empty, BasicPartProperty.Folder)
    member inline _.Yield(sb: SimpleBlock) = [ BasicPartProperty.SimpleBlock sb ]
    member inline _.Yield(cb: ComplexBlock) = [ BasicPartProperty.ComplexBlock cb ]

// Variable Part
[<RequireQualifiedAccess>]
type VariablePartProperty =
    | SimpleBlock of SimpleBlock
    | ComplexBlock of ComplexBlock

    static member Folder (vp: VariablePart) (prop: VariablePartProperty) =
        let part = vp.Value

        (match prop with
         | SimpleBlock sb ->
             { part with
                 SimpleBlocks = sb :: part.SimpleBlocks }
         | ComplexBlock cb ->
             { part with
                 ComplexBlocks = cb :: part.ComplexBlocks })
        |> VariablePart

type VariablePartBuilder() =
    inherit DSLBuilder<VariablePart, VariablePartProperty>(VariablePart.Empty, VariablePartProperty.Folder)
    member inline _.Yield(sb: SimpleBlock) = [ VariablePartProperty.SimpleBlock sb ]

    member inline _.Yield(cb: ComplexBlock) =
        [ VariablePartProperty.ComplexBlock cb ]

// Semester
[<RequireQualifiedAccess>]
type SemesterProperty =
    | Number of int
    | BasicPart of BasicPart
    | VariablePart of VariablePart

    static member Folder (semester: Semester) (prop: SemesterProperty) =
        match prop with
        | Number n -> { semester with Number = n }
        // TODO: think about appending
        | BasicPart bp -> { semester with BasicPart = bp }
        | VariablePart vp -> { semester with VariablePart = vp }

type SemesterBuilder() =
    inherit DSLBuilder<Semester, SemesterProperty>(Semester.Empty, SemesterProperty.Folder)

    member inline _.Yield(bp: BasicPart) = [ SemesterProperty.BasicPart bp ]
    member inline _.Yield(vp: VariablePart) = [ SemesterProperty.VariablePart vp ]

    [<CustomOperation("number")>]
    member inline this.SetNumber(props, n) =
        this.Combine(SemesterProperty.Number n, props)

// Plan
[<RequireQualifiedAccess>]
type PlanProperty =
    | Name of string
    | EnglishName of string
    | StudyLevel of StudyLevel
    | Specialty of string
    | LanguagesOfInstruction of LanguageOfInstruction list
    | YearOfAdmission of int
    | Code of int
    | Competencies of Map<string, string>
    | Semesters of Semester list

    static member Folder (plan: Plan) (prop: PlanProperty) =
        match prop with
        | Name name -> { plan with Name = name }
        | EnglishName name -> { plan with EnglishName = name }
        | StudyLevel level -> { plan with StudyLevel = level }
        | Specialty specialty -> { plan with Specialty = specialty }
        | LanguagesOfInstruction langs ->
            { plan with
                LanguagesOfInstruction = langs }
        | YearOfAdmission year -> { plan with YearOfAdmission = year }
        | Code code -> { plan with Code = code }
        | Competencies comps -> { plan with Competencies = comps }
        | Semesters semesters -> { plan with Semesters = semesters }

type PlanBuilder() =
    inherit DSLBuilder<Plan, PlanProperty>(Plan.Empty, PlanProperty.Folder)

    [<CustomOperation("name")>]
    member inline this.SetName(props, name) =
        this.Combine(PlanProperty.Name name, props)

    [<CustomOperation("englishName")>]
    member inline this.SetEnglishName(props, name) =
        this.Combine(PlanProperty.EnglishName name, props)

    [<CustomOperation("studyLevel")>]
    member inline this.SetStudyLevel(props, level) =
        this.Combine(PlanProperty.StudyLevel level, props)

    [<CustomOperation("specialty")>]
    member inline this.SetSpecialty(props, specialty) =
        this.Combine(PlanProperty.Specialty specialty, props)

    [<CustomOperation("languagesOfInstruction")>]
    member inline this.SetLanguagesOfInstruction(props, langs) =
        this.Combine(PlanProperty.LanguagesOfInstruction langs, props)

    [<CustomOperation("yearOfAdmission")>]
    member inline this.SetYearOfAdmission(props, year) =
        this.Combine(PlanProperty.YearOfAdmission year, props)

    [<CustomOperation("code")>]
    member inline this.SetCode(props, code) =
        this.Combine(PlanProperty.Code code, props)

    [<CustomOperation("competencies")>]
    member inline this.SetCompetencies(props, comps) =
        this.Combine(PlanProperty.Competencies comps, props)

    [<CustomOperation("semesters")>]
    member inline this.SetSemesters(props, semesters) =
        this.Combine(PlanProperty.Semesters semesters, props)

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
    { SimpleBlock.Empty with
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

// Complex Block
[<RequireQualifiedAccess>]
type ComplexBlockProperty =
    | Name of string
    | Track of string * SimpleBlock list

    static member Folder (cb: ComplexBlock) (prop: ComplexBlockProperty) =
        match prop with
        | Name n -> { cb with Name = n }
        | Track(name, blocks) ->
            { cb with
                Tracks = cb.Tracks |> Map.add name blocks }

type ComplexBlockBuilder() =
    inherit DSLBuilder<ComplexBlock, ComplexBlockProperty>(ComplexBlock.Empty, ComplexBlockProperty.Folder)

    [<CustomOperation("name")>]
    member inline this.SetName(props: ComplexBlockProperty list, name) =
        this.Combine(ComplexBlockProperty.Name name, props)

    [<CustomOperation("track")>]
    member inline this.SetTrack(props: ComplexBlockProperty list, name, blocks) =
        this.Combine(ComplexBlockProperty.Track(name, blocks), props)
