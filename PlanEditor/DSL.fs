module PlanEditor.DSL

open PlanEditor.Types

// Парсер строки WorkHours в WorkHoursDistribution
let parseWorkHours (s: string) : WorkHoursDistribution =
    let nums =
        s.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.map System.Int32.Parse
        |> Array.toList

    match nums with
    | [l; se; c; p; lb; col; cc; ia; gi; wtp; wt; wmm; cci; ma; tiw] ->
        { Lecture = l
          Seminar = se
          Consultation = c
          Practical = p
          Lab = lb
          Colloquium = col
          CurrentControl = cc
          InterimAssessment = ia
          GuidedIndependent = gi
          WithTeacherPresence = wtp
          WithTeacher = wt
          WithMethodologicalMaterials = wmm
          CurrentControlIndependent = cci
          MidtermAssessment = ma
          TotalIndependentWork = tiw }
    | _ -> failwith "WorkHours string must contain exactly 15 integers"

let workHoursFromString = parseWorkHours

type WorkHoursBuilder() =
    member _.Yield(_) = emptyWorkHours

    [<CustomOperation("lecture")>]
    member _.Lecture(state: WorkHoursDistribution, v: int) = { state with Lecture = v }

    [<CustomOperation("seminar")>]
    member _.Seminar(state, v: int) = { state with Seminar = v }

    [<CustomOperation("consultation")>]
    member _.Consultation(state, v: int) = { state with Consultation = v }

    [<CustomOperation("practical")>]
    member _.Practical(state, v: int) = { state with Practical = v }

    [<CustomOperation("lab")>]
    member _.Lab(state, v: int) = { state with Lab = v }

    [<CustomOperation("colloquium")>]
    member _.Colloquium(state, v: int) = { state with Colloquium = v }

    [<CustomOperation("currentControl")>]
    member _.CurrentControl(state, v: int) = { state with CurrentControl = v }

    [<CustomOperation("interimAssessment")>]
    member _.InterimAssessment(state, v: int) = { state with InterimAssessment = v }

    [<CustomOperation("guidedIndependent")>]
    member _.GuidedIndependent(state, v: int) = { state with GuidedIndependent = v }

    [<CustomOperation("withTeacherPresence")>]
    member _.WithTeacherPresence(state, v: int) = { state with WithTeacherPresence = v }

    [<CustomOperation("withTeacher")>]
    member _.WithTeacher(state, v: int) = { state with WithTeacher = v }

    [<CustomOperation("withMethodologicalMaterials")>]
    member _.WithMethodologicalMaterials(state, v: int) = { state with WithMethodologicalMaterials = v }

    [<CustomOperation("currentControlIndependent")>]
    member _.CurrentControlIndependent(state, v: int) = { state with CurrentControlIndependent = v }

    [<CustomOperation("midtermAssessment")>]
    member _.MidtermAssessment(state, v: int) = { state with MidtermAssessment = v }

    [<CustomOperation("totalIndependentWork")>]
    member _.TotalIndependentWork(state, v: int) = { state with TotalIndependentWork = v }

let workHours = WorkHoursBuilder()

type ImplementationBuilder() =
    member _.Yield(_) = emptyImplementation

    [<CustomOperation("semester")>]
    member _.Semester(state, s: int) = { state with Semester = s }

    [<CustomOperation("laborIntensity")>]
    member _.LaborIntensity(state, ze: int) = { state with LaborIntensity = ze }

    [<CustomOperation("blockCode")>]
    member _.BlockCode(state, bc: FgosBlockCode) = { state with BlockCode = bc }

    [<CustomOperation("realization")>]
    member _.Realization(state, r: string) = { state with Realization = r }

    [<CustomOperation("trajectory")>]
    member _.Trajectory(state, t: string) = { state with Trajectory = t }

    [<CustomOperation("monitoring")>]
    member _.Monitoring(state, mts: MonitoringType list) = { state with MonitoringTypes = mts }

    [<CustomOperation("workHours")>]
    member _.WorkHours(state, wh: WorkHoursDistribution) = { state with WorkHours = wh }

    [<CustomOperation("workHoursFromString")>]
    member _.WorkHoursFromString(state, s: string) = { state with WorkHours = parseWorkHours s }

    [<CustomOperation("competences")>]
    member _.Competences(state, cs: Competence list) = { state with Competences = cs }

let implementation = ImplementationBuilder()

type CourseBuilder() =

    member _.Yield(_) = emptyCourse

    [<CustomOperation("code")>]
    member _.Code(state: Course, c: string) =
        { state with Code = c }

    [<CustomOperation("russianName")>]
    member _.RussianName(state: Course, name: string) =
        { state with RussianName = name }

    [<CustomOperation("englishName")>]
    member _.EnglishName(state: Course, name: string) =
        { state with EnglishName = name }

    [<CustomOperation("courseType")>]
    member _.CourseType(state: Course, t: CourseType) =
        { state with Type = t }

    [<CustomOperation("implementations")>]
    member _.Implementations(state: Course, impls: Implementation list) =
        { state with Implementations = impls }

    [<CustomOperation("electiveBlock")>]
    member _.AddElectiveBlock(state: Course, entry: ElectivesBlockEntry) =
        { state with ElectivesBlock = entry :: state.ElectivesBlock }

let course = CourseBuilder()
