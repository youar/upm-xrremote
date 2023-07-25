/*

            ServerReceiver.cs
                ServerReceiver : CustomNdiReceiver
                class information specific to the ServerReceiver

                    create override method like unpackMetadata();


        13  and then we gotta paint         a beautiful picture with the UI information that gets unpacked from the ClientSender.Metadata package thingy


    TODO:
        - Re-implement debug flags & reenable errors
        - Decouple planes and input classes from sender/receiver class
            - pass data using EventArgs
        - Fix singleton problems so we don't have to rely on script execution order
        - Finish Bidirectional Sender
            - Client packet
            - Send packet on OnCameraRender?
        - Too many canvases?
            - Check if they block raycasting
            - Maybe switch from FindObjectByType to finding by name
        - Planes
            -Refactor to provider format (see ArkitSender project)
        - Cysharp serializer
        - Rename SerializableXRPlaneNdi
        - PlaneSender fix
            - OnConnect send all data


    DONE:

                
            CustomNdiSender.cs : MonoBehavior
                CustomNdiSender
                Class information and properties and stuff
                
                    empty method like setMetadata();

            CustomNdiReceiver.cs : MonoBehavior
                CustomNdiReceiver
                class information and properties and stuff 'n things

                    empty method like unpackMetadata();

            ---------------------------------------------------------

            ServerSender.cs
                ServerSender : CustomNdiSender
                class information specific to the ServerSender

                    create override method like SetMetadata();

            ServerReceiver.cs
                ServerReceiver : CustomNdiReceiver
                class information specific to the ServerReceiver

                    create override method like unpackMetadata();

            ---------------------------------------------------------
            ClientSender.cs
                ClientSender : CustomNdiSender
                class information specific to the ClientSender

                    create override method like SetMetadata();

            ClientReceiver.cs  
                ClientReceiver : CustomNdiReceiver
                class information specific to the ClientReceiver

                    create override method like unpackMetadata();

            ---------------------------------------------------------

            ServerRemotePacket -
            things the server sends that the client needs to do stuff with
            (we're going to eventually have to send input from touching the screen on the Server to the Client)

            ClientRemotePacket - 
            things the client sends that the server needs to do stuff with (i.e. render the ui over the camera)

            ---------------------------------------------------------

        1   set the aspect ratio fitter     to respond dynamically to the w x h dimensions earlier in the function (idk if that makes sense)

        3   we also have to create a        function that encapsulates the metadata that we can call every time we're sending metadata both in the ServerSender and ClientSender places
        4   and then we gotta               implement that function in the ClientSender and in the ServerSender
        5   also we gotta unpack            the metadata that the ServerReceiver receives
        
        9   we can also                     remove CUSTOM from things that don't need custom involved in their names
*/

