using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalEngineer.Drawing {
    //This is borrowed from mechjeb solely for debugging purposes. (OK I lied)
    class DebugDrawing {

        static Material _material;
        static Material material {
            get
            {
                if (_material == null) _material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
                return _material;
            }
        }

        public static void DrawGroundMarker(CelestialBody body, double latitude, double longitude, Color c, bool map, double rotation = 0, double radius = 0) {
            Vector3d up = body.GetSurfaceNVector(latitude, longitude);
            var height = 1 + body.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(longitude, Vector3d.down) * QuaternionD.AngleAxis(latitude, Vector3d.forward) * Vector3d.right);
            if (height < body.Radius + 1) { height = body.Radius + 1; }
            Vector3d center = body.position + height * up;
            Vector3d north = Vector3d.Exclude(up, body.transform.up).normalized;

            if (map) { //grr
                Vector3d camPos = ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position);
                if (IsOccluded(center, body, camPos)) return;
            }

            if (radius <= 0) { radius = map ? body.Radius / 50 : 15; }

            List<Vector3d> Verts = new List<Vector3d>();

            int num = 64;

            for (int i = 0; i <= num; i++) {
                Verts.Add(center + radius * 0.85 * (QuaternionD.AngleAxis(rotation + i * 360 / num, up) * north));
                Verts.Add(center + radius * (QuaternionD.AngleAxis(rotation + i * 360 / num, up) * north));
            }
            GLTriangleStrip(Verts, c, map);

            for (int i = 0; i <= num; i++) {
                Verts.Add(center + radius * 0.50 * (QuaternionD.AngleAxis(rotation + i * 360 / num, up) * north));
                Verts.Add(center + radius * 0.55 * (QuaternionD.AngleAxis(rotation + i * 360 / num, up) * north));
            }
            GLTriangleStrip(Verts, c, map);


            for (int i = 0; i <= num; i++) {
                Verts.Add(center + radius * 0.175 * (QuaternionD.AngleAxis(rotation + i * 360 / num, up) * north));
                Verts.Add(center + radius * 0.2 * (QuaternionD.AngleAxis(rotation + i * 360 / num, up) * north));
            }
            GLTriangleStrip(Verts, c, map);

        }

        public static void GLTriangleStrip(List<Vector3d> Verts, Color c, bool map) {
            GL.PushMatrix();
            material.SetPass(0);

            if (map) { 
                //I do this under protest because I can't figure out how to set up a ztest=always shader.
                //its only needed because the map planet meshes are awful and the actual point is frequently way below the 'surface'
                //To be fair this is how KSP renders most map nodes.
                GL.LoadOrtho();
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(c);
                foreach (var item in Verts) {
                    Vector3d v = item;
                    Vector3 screenPoint = PlanetariumCamera.Camera.WorldToViewportPoint(ScaledSpace.LocalToScaledSpace(v));
                    GL.Vertex3(screenPoint.x, screenPoint.y, 0);
                }
                GL.End();
            } else { //this is how it should be, render the thing in world coords with depth.
                GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(c);
                foreach (var item in Verts) {
                    Vector3d v = item;
                    GL.Vertex3((float)v.x, (float)v.y, (float)v.z);
                }
                GL.End();
            }

            GL.PopMatrix();

        }


        public static void GLPixelLine(Vector3d worldPosition1, Vector3d worldPosition2, bool map) {
            Vector3 screenPoint1, screenPoint2;
            if (map) {
                screenPoint1 = PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(worldPosition1));
                screenPoint2 = PlanetariumCamera.Camera.WorldToScreenPoint(ScaledSpace.LocalToScaledSpace(worldPosition2));
            } else {
                screenPoint1 = FlightCamera.fetch.mainCamera.WorldToScreenPoint(worldPosition1);
                screenPoint2 = FlightCamera.fetch.mainCamera.WorldToScreenPoint(worldPosition2);
            }

            if (screenPoint1.z > 0 && screenPoint2.z > 0) {
                GL.Vertex3(screenPoint1.x, screenPoint1.y, 0);
                GL.Vertex3(screenPoint2.x, screenPoint2.y, 0);
            }
        }


        //Tests if byBody occludes worldPosition, from the perspective of the planetarium camera
        // https://cesiumjs.org/2013/04/25/Horizon-culling/
        public static bool IsOccluded(Vector3d worldPosition, CelestialBody byBody, Vector3d camPos) {
            Vector3d VC = (byBody.position - camPos) / (byBody.Radius - 100);
            Vector3d VT = (worldPosition - camPos) / (byBody.Radius - 100);

            double VT_VC = Vector3d.Dot(VT, VC);

            // In front of the horizon plane
            if (VT_VC < VC.sqrMagnitude - 1) return false;

            return VT_VC * VT_VC / VT.sqrMagnitude > VC.sqrMagnitude - 1;
        }

        //If dashed = false, draws 0-1-2-3-4-5...
        //If dashed = true, draws 0-1 2-3 4-5...
        public static void DrawPath(CelestialBody mainBody, List<Vector3d> points, Color c, bool map, bool dashed = false) {
            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);
            GL.Color(c);

            Vector3d camPos = map ? ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position) : (Vector3d)FlightCamera.fetch.mainCamera.transform.position;

            int step = (dashed ? 2 : 1);
            for (int i = 0; i < points.Count - 1; i += step) {
                if (!IsOccluded(points[i], mainBody, camPos) && !IsOccluded(points[i + 1], mainBody, camPos)) {
                    GLPixelLine(points[i], points[i + 1], map);
                }
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}
