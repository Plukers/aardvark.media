﻿module App

open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering
open Model

let initialCamera = { 
        CameraController.initial with 
            view = CameraView.lookAt (V3d.III * 3.0) V3d.OOO V3d.OOI
    }

let update (model : Model) (msg : Message) =
    match msg with
        | Camera m -> 
            { model with cameraState = CameraController.update model.cameraState m }

        | CenterScene -> 
            { model with cameraState = initialCamera }

        | UpdateConfig cfg ->
            { model with dockConfig = cfg; past = Some model }

        | Undo ->
            match model.past with
                | Some p -> { p with future = Some model; cameraState = model.cameraState }
                | None -> model

        | Redo ->
            match model.future with
                | Some f -> { f with past = Some model; cameraState = model.cameraState }
                | None -> model

let viewScene (model : MModel) =
    Sg.box (Mod.constant C4b.Green) (Mod.constant Box3d.Unit)
     |> Sg.shader {
            do! DefaultSurfaces.trafo
            do! DefaultSurfaces.vertexColor
            do! DefaultSurfaces.simpleLighting
        }

let view (model : MModel) =


    let renderControl =
        CameraController.controlledControl model.cameraState Camera (Frustum.perspective 60.0 0.1 100.0 1.0 |> Mod.constant) 
                    (AttributeMap.ofList [ style "width: 100%; height:100%;data-samples:8"]) 
                    (viewScene model)

    page (fun request -> 
        match Map.tryFind "page" request.queryParams with
            | Some "controls" -> 
                body [] [
                    button [onClick (fun _ -> CenterScene)] [text "Center Scene"]
                ]

            | Some "render" -> 
                body [] [
                    renderControl
                ]

            | Some "meta" ->
                body [] [
                    button [onClick (fun _ -> Undo)] [text "Undo"]
                ]

            | Some other ->
                let msg = sprintf "Unknown page: %A" other
                body [] [
                    div [style "color: white; font-size: large; background-color: red; width: 100%; height: 100%"] [text msg]
                ]  

            | None -> 
                model.dockConfig |> docking [
                    style "width:100%;height:100%;"
                    onLayoutChanged UpdateConfig
                ]
    )

let threads (model : Model) = 
    CameraController.threads model.cameraState |> ThreadPool.map Camera

let app =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
                past = None
                future = None
                cameraState = initialCamera
                dockConfig =
                    config (
                        horizontal 10.0 [
                            element { id "render"; title "Render View"; weight 20 }
                            vertical 5.0 [
                                element { id "controls"; title "Controls"; weight 5 }
                                element { id "meta"; title "Layout Controls"; weight 5 }
                            ]
                        ]
                    )
            }
        update = update 
        view = view
    }
