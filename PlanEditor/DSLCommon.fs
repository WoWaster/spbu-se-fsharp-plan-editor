module PlanEditor.DSLCommon

// TODO: if it even possible, use constraints like this when
// 'Base: (static member Empty: 'Base) and 'Prop: (static member Folder: 'Base -> 'Prop -> 'Base)
// Or maybe https://aka.ms/fsharp-iwsams can be used

type DSLBuilder<'Base, 'Prop>(empty: 'Base, folder: 'Base -> 'Prop -> 'Base) =
    member inline _.Yield(()) = []
    member inline _.Delay(f: unit -> 'Prop list) = f ()
    // member inline _.Delay(f: unit -> 'T) = [ f () ]

    member inline _.Combine(newProp: 'Prop, props: 'Prop list) = newProp :: props
    member inline _.Combine(newProps: 'Prop list, props: 'Prop list) = newProps @ props

    member inline this.For(props: 'Prop list, f: unit -> 'Prop list) = this.Combine(props, f ())

    member inline this.For(prop: 'Prop, f: unit -> 'Prop list) = this.Combine(prop, f ())

    member inline _.For(prop: 'Prop, f: unit -> 'Prop) = [ prop; f () ]

    member _.Run(props: 'Prop list) = props |> List.fold folder empty

    member x.Run(prop: 'Prop) = x.Run [ prop ]
