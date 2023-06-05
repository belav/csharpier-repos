// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.Drawing.Text;
using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

namespace System.Drawing
{
    partial
    // Raw function imports for gdiplus
    internal static class SafeNativeMethods
    {
        partial internal static unsafe class Gdip
        {
            private const string LibraryName = "gdiplus.dll";

            // Imported functions
            [LibraryImport(LibraryName)]
            partial private static int GdiplusStartup(
                out IntPtr token,
                in StartupInputEx input,
                out StartupOutput output
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBeginContainer(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                ref RectangleF dstRect,
                ref RectangleF srcRect,
                GraphicsUnit unit,
                out int state
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBeginContainer2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int state
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBeginContainerI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                ref Rectangle dstRect,
                ref Rectangle srcRect,
                GraphicsUnit unit,
                out int state
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEndContainer(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int state
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateAdjustableArrowCap(
                float height,
                float width,
                [MarshalAs(UnmanagedType.Bool)] bool isFilled,
                out IntPtr adjustableArrowCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetAdjustableArrowCapHeight(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                out float height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetAdjustableArrowCapHeight(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                float height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetAdjustableArrowCapWidth(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                float width
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetAdjustableArrowCapWidth(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                out float width
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetAdjustableArrowCapMiddleInset(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                float middleInset
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetAdjustableArrowCapMiddleInset(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                out float middleInset
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetAdjustableArrowCapFillState(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                [MarshalAs(UnmanagedType.Bool)] bool fillState
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetAdjustableArrowCapFillState(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef adjustableArrowCap,
                [MarshalAs(UnmanagedType.Bool)] out bool fillState
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCustomLineCapType(
                IntPtr customCap,
                out CustomLineCapType capType
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateCustomLineCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fillpath,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef strokepath,
                LineCap baseCap,
                float baseInset,
                out IntPtr customCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteCustomLineCap(IntPtr customCap);

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteCustomLineCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneCustomLineCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                out IntPtr clonedCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetCustomLineCapStrokeCaps(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                LineCap startCap,
                LineCap endCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCustomLineCapStrokeCaps(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                out LineCap startCap,
                out LineCap endCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetCustomLineCapStrokeJoin(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                LineJoin lineJoin
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCustomLineCapStrokeJoin(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                out LineJoin lineJoin
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetCustomLineCapBaseCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                LineCap baseCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCustomLineCapBaseCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                out LineCap baseCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetCustomLineCapBaseInset(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                float inset
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCustomLineCapBaseInset(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                out float inset
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetCustomLineCapWidthScale(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                float widthScale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCustomLineCapWidthScale(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap,
                out float widthScale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePathIter(
                out IntPtr pathIter,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeletePathIter(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterNextSubpath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int resultCount,
                out int startIndex,
                out int endIndex,
                [MarshalAs(UnmanagedType.Bool)] out bool isClosed
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterNextSubpathPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int resultCount,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                [MarshalAs(UnmanagedType.Bool)] out bool isClosed
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterNextPathType(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int resultCount,
                out byte pathType,
                out int startIndex,
                out int endIndex
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterNextMarker(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int resultCount,
                out int startIndex,
                out int endIndex
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterNextMarkerPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int resultCount,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterGetCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterGetSubpathCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterHasCurve(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                [MarshalAs(UnmanagedType.Bool)] out bool hasCurve
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterRewind(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterEnumerate(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int resultCount,
                PointF* points,
                byte* types,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPathIterCopyData(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pathIter,
                out int resultCount,
                PointF* points,
                byte* types,
                int startIndex,
                int endIndex
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateHatchBrush(
                int hatchstyle,
                int forecol,
                int backcol,
                out IntPtr brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetHatchStyle(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int hatchstyle
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetHatchForegroundColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int forecol
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetHatchBackgroundColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int backcol
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneBrush(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out IntPtr clonebrush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateLineBrush(
                ref PointF point1,
                ref PointF point2,
                int color1,
                int color2,
                WrapMode wrapMode,
                out IntPtr lineGradient
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateLineBrushI(
                ref Point point1,
                ref Point point2,
                int color1,
                int color2,
                WrapMode wrapMode,
                out IntPtr lineGradient
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateLineBrushFromRect(
                ref RectangleF rect,
                int color1,
                int color2,
                LinearGradientMode lineGradientMode,
                WrapMode wrapMode,
                out IntPtr lineGradient
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateLineBrushFromRectI(
                ref Rectangle rect,
                int color1,
                int color2,
                LinearGradientMode lineGradientMode,
                WrapMode wrapMode,
                out IntPtr lineGradient
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateLineBrushFromRectWithAngle(
                ref RectangleF rect,
                int color1,
                int color2,
                float angle,
                [MarshalAs(UnmanagedType.Bool)] bool isAngleScaleable,
                WrapMode wrapMode,
                out IntPtr lineGradient
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateLineBrushFromRectWithAngleI(
                ref Rectangle rect,
                int color1,
                int color2,
                float angle,
                [MarshalAs(UnmanagedType.Bool)] bool isAngleScaleable,
                WrapMode wrapMode,
                out IntPtr lineGradient
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLineColors(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int color1,
                int color2
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineColors(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int[] colors
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out RectangleF gprectf
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineGammaCorrection(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                [MarshalAs(UnmanagedType.Bool)] out bool useGammaCorrection
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLineGammaCorrection(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                [MarshalAs(UnmanagedType.Bool)] bool useGammaCorrection
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLineSigmaBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float focus,
                float scale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLineLinearBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float focus,
                float scale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineBlendCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                IntPtr blend,
                IntPtr positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLineBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                IntPtr blend,
                IntPtr positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLinePresetBlendCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLinePresetBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                IntPtr blend,
                IntPtr positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLinePresetBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                IntPtr blend,
                IntPtr positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLineWrapMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int wrapMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineWrapMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int wrapMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipResetLineTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipMultiplyLineTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetLineTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslateLineTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float dx,
                float dy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipScaleLineTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float sx,
                float sy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRotateLineTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float angle,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePathGradient(
                PointF* points,
                int count,
                WrapMode wrapMode,
                out IntPtr brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePathGradientI(
                Point* points,
                int count,
                WrapMode wrapMode,
                out IntPtr brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePathGradientFromPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                out IntPtr brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientCenterColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int color
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientCenterColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int color
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientSurroundColorsWithCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int[] color,
                ref int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientSurroundColorsWithCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int[] argb,
                ref int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientCenterPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out PointF point
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientCenterPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                ref PointF point
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out RectangleF gprectf
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientPointCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientSurroundColorCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientBlendCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float[] blend,
                float[] positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                IntPtr blend,
                IntPtr positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientPresetBlendCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientPresetBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int[] blend,
                float[] positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientPresetBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int[] blend,
                float[] positions,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientSigmaBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float focus,
                float scale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientLinearBlend(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float focus,
                float scale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientWrapMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int wrapmode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientWrapMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int wrapmode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipResetPathGradientTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipMultiplyPathGradientTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslatePathGradientTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float dx,
                float dy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipScalePathGradientTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float sx,
                float sy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRotatePathGradientTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float angle,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathGradientFocusScales(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float[] xScale,
                float[] yScale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathGradientFocusScales(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float xScale,
                float yScale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateImageAttributes(out IntPtr imageattr);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneImageAttributes(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                out IntPtr cloneImageattr
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDisposeImageAttributes(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesColorMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(ColorMatrix.PinningMarshaller))]
#endif
                ColorMatrix? colorMatrix,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(ColorMatrix.PinningMarshaller))]
#endif
                ColorMatrix? grayMatrix,
                ColorMatrixFlag flags
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesThreshold(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag,
                float threshold
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesGamma(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag,
                float gamma
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesNoOp(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesColorKeys(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag,
                int colorLow,
                int colorHigh
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesOutputChannel(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag,
                ColorChannelFlag flags
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipSetImageAttributesOutputChannelColorProfile(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag,
                string colorProfileFilename
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesRemapTable(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                ColorAdjustType type,
                [MarshalAs(UnmanagedType.Bool)] bool enableFlag,
                int mapSize,
                IntPtr map
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImageAttributesWrapMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                int wrapmode,
                int argb,
                [MarshalAs(UnmanagedType.Bool)] bool clamp
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageAttributesAdjustedPalette(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattr,
                IntPtr palette,
                ColorAdjustType type
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageDecodersSize(out int numDecoders, out int size);

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageDecoders(
                int numDecoders,
                int size,
                IntPtr decoders
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageEncodersSize(out int numEncoders, out int size);

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageEncoders(
                int numEncoders,
                int size,
                IntPtr encoders
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateSolidFill(int color, out IntPtr brush);

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetSolidFillColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int color
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetSolidFillColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int color
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateTexture(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
                int wrapmode,
                out IntPtr texture
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateTexture2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
                int wrapmode,
                float x,
                float y,
                float width,
                float height,
                out IntPtr texture
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateTextureIA(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageAttrib,
                float x,
                float y,
                float width,
                float height,
                out IntPtr texture
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateTexture2I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
                int wrapmode,
                int x,
                int y,
                int width,
                int height,
                out IntPtr texture
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateTextureIAI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageAttrib,
                int x,
                int y,
                int width,
                int height,
                out IntPtr texture
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetTextureTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetTextureTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipResetTextureTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipMultiplyTextureTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslateTextureTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float dx,
                float dy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipScaleTextureTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float sx,
                float sy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRotateTextureTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float angle,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetTextureWrapMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int wrapMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetTextureWrapMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out int wrapMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetTextureImage(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                out IntPtr image
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFontCollectionFamilyCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontCollection,
                out int numFound
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFontCollectionFamilyList(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontCollection,
                int numSought,
                IntPtr[] gpfamilies,
                out int numFound
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneFontFamily(
                IntPtr fontfamily,
                out IntPtr clonefontfamily
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipCreateFontFamilyFromName(
                string name,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontCollection,
                out IntPtr FontFamily
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetGenericFontFamilySansSerif(out IntPtr fontfamily);

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetGenericFontFamilySerif(out IntPtr fontfamily);

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetGenericFontFamilyMonospace(out IntPtr fontfamily);

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteFontFamily(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontFamily
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipGetFamilyName(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef family,
                char* name,
                int language
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsStyleAvailable(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef family,
                FontStyle style,
                out int isStyleAvailable
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetEmHeight(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef family,
                FontStyle style,
                out int EmHeight
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCellAscent(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef family,
                FontStyle style,
                out int CellAscent
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCellDescent(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef family,
                FontStyle style,
                out int CellDescent
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLineSpacing(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef family,
                FontStyle style,
                out int LineSpaceing
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipNewInstalledFontCollection(out IntPtr fontCollection);

            [LibraryImport(LibraryName)]
            partial internal static int GdipNewPrivateFontCollection(out IntPtr fontCollection);

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeletePrivateFontCollection(ref IntPtr fontCollection);

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipPrivateAddFontFile(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontCollection,
                string filename
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPrivateAddMemoryFont(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontCollection,
                IntPtr memory,
                int length
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateFont(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontFamily,
                float emSize,
                FontStyle style,
                GraphicsUnit unit,
                out IntPtr font
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateFontFromDC(IntPtr hdc, ref IntPtr font);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneFont(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                out IntPtr cloneFont
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteFont(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFamily(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                out IntPtr family
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFontStyle(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                out FontStyle style
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFontSize(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                out float size
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFontHeight(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out float size
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFontHeightGivenDPI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                float dpi,
                out float size
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetFontUnit(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                out GraphicsUnit unit
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetLogFontW(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                ref Interop.User32.LOGFONT lf
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePen1(
                int argb,
                float width,
                int unit,
                out IntPtr pen
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePen2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float width,
                int unit,
                out IntPtr pen
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipClonePen(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out IntPtr clonepen
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeletePen(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef Pen
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PenAlignment penAlign
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out PenAlignment penAlign
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenWidth(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float width
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenWidth(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float[] width
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenLineCap197819(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int startCap,
                int endCap,
                int dashCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenStartCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int startCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenEndCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int endCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenStartCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int startCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenEndCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int endCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenDashCap197819(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int dashCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenDashCap197819(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int dashCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenLineJoin(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int lineJoin
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenLineJoin(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int lineJoin
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenCustomStartCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenCustomStartCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out IntPtr customCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenCustomEndCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef customCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenCustomEndCap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out IntPtr customCap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenMiterLimit(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float miterLimit
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenMiterLimit(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float[] miterLimit
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipResetPenTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipMultiplyPenTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslatePenTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float dx,
                float dy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipScalePenTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float sx,
                float sy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRotatePenTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float angle,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int argb
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int argb
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenBrushFill(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenBrushFill(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out IntPtr brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenFillType(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int pentype
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenDashStyle(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int dashstyle
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenDashStyle(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int dashstyle
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenDashArray(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef memorydash,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenDashOffset(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float[] dashoffset
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenDashOffset(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float dashoffset
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenDashCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int dashcount
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenDashArray(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float[] memorydash,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenCompoundCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPenCompoundArray(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float[] array,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPenCompoundArray(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float[] array,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetWorldTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipResetWorldTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipMultiplyWorldTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslateWorldTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float dx,
                float dy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipScaleWorldTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float sx,
                float sy,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRotateWorldTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float angle,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetWorldTransform(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetCompositingMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                CompositingMode compositingMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetTextRenderingHint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                TextRenderingHint textRenderingHint
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetTextContrast(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int textContrast
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetInterpolationMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                InterpolationMode interpolationMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCompositingMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out CompositingMode compositingMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetRenderingOrigin(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int x,
                int y
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetRenderingOrigin(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int x,
                out int y
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetCompositingQuality(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                CompositingQuality quality
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetCompositingQuality(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out CompositingQuality quality
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetSmoothingMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                SmoothingMode smoothingMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetSmoothingMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out SmoothingMode smoothingMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPixelOffsetMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                PixelOffsetMode pixelOffsetMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPixelOffsetMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out PixelOffsetMode pixelOffsetMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetTextRenderingHint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out TextRenderingHint textRenderingHint
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetTextContrast(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int textContrast
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetInterpolationMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out InterpolationMode interpolationMode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPageUnit(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out GraphicsUnit unit
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPageScale(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out float scale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPageUnit(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                GraphicsUnit unit
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPageScale(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float scale
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetDpiX(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out float dpi
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetDpiY(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out float dpi
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateMatrix(out IntPtr matrix);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateMatrix2(
                float m11,
                float m12,
                float m21,
                float m22,
                float dx,
                float dy,
                out IntPtr matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateMatrix3(
                ref RectangleF rect,
                PointF* dstplg,
                out IntPtr matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateMatrix3I(
                ref Rectangle rect,
                Point* dstplg,
                out IntPtr matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                out IntPtr cloneMatrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetMatrixElements(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                float m11,
                float m12,
                float m21,
                float m22,
                float dx,
                float dy
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipMultiplyMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix2,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslateMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                float offsetX,
                float offsetY,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipScaleMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                float scaleX,
                float scaleY,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRotateMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                float angle,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipShearMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                float shearX,
                float shearY,
                MatrixOrder order
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipInvertMatrix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTransformMatrixPoints(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                PointF* pts,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTransformMatrixPointsI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                Point* pts,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipVectorTransformMatrixPoints(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                PointF* pts,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipVectorTransformMatrixPointsI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                Point* pts,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static unsafe int GdipGetMatrixElements(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                float* m
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsMatrixInvertible(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsMatrixIdentity(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsMatrixEqual(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix2,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateRegion(out IntPtr region);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateRegionRect(
                ref RectangleF gprectf,
                out IntPtr region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateRegionRectI(
                ref Rectangle gprect,
                out IntPtr region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateRegionPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                out IntPtr region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateRegionRgnData(
                byte[] rgndata,
                int size,
                out IntPtr region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateRegionHrgn(IntPtr hRgn, out IntPtr region);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                out IntPtr cloneregion
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetInfinite(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetEmpty(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCombineRegionRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                ref RectangleF gprectf,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCombineRegionRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                ref Rectangle gprect,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCombineRegionPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCombineRegionRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region2,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslateRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                float dx,
                float dy
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslateRegionI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                int dx,
                int dy
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTransformRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetRegionBounds(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out RectangleF gprectf
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetRegionHRgn(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out IntPtr hrgn
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsEmptyRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsInfiniteRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsEqualRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region2,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetRegionDataSize(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                out int bufferSize
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetRegionData(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                byte[] regionData,
                int bufferSize,
                out int sizeFilled
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisibleRegionPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                float X,
                float Y,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisibleRegionPointI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                int X,
                int Y,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisibleRegionRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                float X,
                float Y,
                float width,
                float height,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisibleRegionRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                int X,
                int Y,
                int width,
                int height,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int boolean
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetRegionScansCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                out int count,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetRegionScans(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                RectangleF* rects,
                out int count,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateFromHDC(IntPtr hdc, out IntPtr graphics);

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetClipGraphics(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef srcgraphics,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetClipRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float x,
                float y,
                float width,
                float height,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetClipRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int x,
                int y,
                int width,
                int height,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetClipPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetClipRegion(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region,
                CombineMode mode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipResetClip(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTranslateClip(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float dx,
                float dy
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetClip(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef region
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetClipBounds(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out RectangleF rect
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsClipEmpty(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetVisibleClipBounds(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out RectangleF rect
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisibleClipEmpty(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisiblePoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float x,
                float y,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisiblePointI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int x,
                int y,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisibleRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                float x,
                float y,
                float width,
                float height,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisibleRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int x,
                int y,
                int width,
                int height,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipFlush(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                FlushIntention intention
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetDC(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out IntPtr hdc
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatMeasurableCharacterRanges(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                int rangeCount,
                CharacterRange[] range
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateStringFormat(
                StringFormatFlags options,
                int language,
                out IntPtr format
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipStringFormatGetGenericDefault(out IntPtr format);

            [LibraryImport(LibraryName)]
            partial internal static int GdipStringFormatGetGenericTypographic(out IntPtr format);

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteStringFormat(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneStringFormat(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out IntPtr newFormat
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatFlags(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                StringFormatFlags options
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatFlags(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out StringFormatFlags result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatAlign(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                StringAlignment align
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatAlign(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out StringAlignment align
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatLineAlign(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                StringAlignment align
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatLineAlign(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out StringAlignment align
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatHotkeyPrefix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                HotkeyPrefix hotkeyPrefix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatHotkeyPrefix(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out HotkeyPrefix hotkeyPrefix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatTabStops(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                float firstTabOffset,
                int count,
                float[] tabStops
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatTabStops(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                int count,
                out float firstTabOffset,
                float[] tabStops
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatTabStopCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatMeasurableCharacterRangeCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatTrimming(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                StringTrimming trimming
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatTrimming(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out StringTrimming trimming
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetStringFormatDigitSubstitution(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                int langID,
                StringDigitSubstitute sds
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetStringFormatDigitSubstitution(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format,
                out int langID,
                out StringDigitSubstitute sds
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageDimension(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out float width,
                out float height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageWidth(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out int width
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageHeight(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out int height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageHorizontalResolution(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out float horzRes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageVerticalResolution(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out float vertRes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageFlags(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out int flags
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageRawFormat(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                ref Guid format
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImagePixelFormat(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out PixelFormat format
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipImageGetFrameCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                ref Guid dimensionID,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipImageSelectActiveFrame(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                ref Guid dimensionID,
                int frameIndex
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipImageRotateFlip(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int rotateFlipType
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetAllPropertyItems(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                uint totalBufferSize,
                uint numProperties,
                PropertyItemInternal* allItems
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPropertyCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out uint numOfProperty
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPropertyIdList(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                uint numOfProperty,
                int* list
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPropertyItem(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int propid,
                uint propSize,
                PropertyItemInternal* buffer
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPropertyItemSize(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int propid,
                out uint size
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPropertySize(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out uint totalBufferSize,
                out uint numProperties
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRemovePropertyItem(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int propid
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPropertyItem(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                PropertyItemInternal* item
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageType(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out int type
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageType(IntPtr image, out int type);

            [LibraryImport(LibraryName)]
            partial internal static int GdipDisposeImage(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDisposeImage(IntPtr image);

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipCreateBitmapFromFile(
                string filename,
                out IntPtr bitmap
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipCreateBitmapFromFileICM(
                string filename,
                out IntPtr bitmap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateBitmapFromScan0(
                int width,
                int height,
                int stride,
                int format,
                IntPtr scan0,
                out IntPtr bitmap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateBitmapFromGraphics(
                int width,
                int height,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out IntPtr bitmap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateBitmapFromHBITMAP(
                IntPtr hbitmap,
                IntPtr hpalette,
                out IntPtr bitmap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateBitmapFromHICON(IntPtr hicon, out IntPtr bitmap);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateBitmapFromResource(
                IntPtr hresource,
                IntPtr name,
                out IntPtr bitmap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateHBITMAPFromBitmap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef nativeBitmap,
                out IntPtr hbitmap,
                int argbBackground
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateHICONFromBitmap(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef nativeBitmap,
                out IntPtr hicon
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneBitmapArea(
                float x,
                float y,
                float width,
                float height,
                int format,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef srcbitmap,
                out IntPtr dstbitmap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneBitmapAreaI(
                int x,
                int y,
                int width,
                int height,
                int format,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef srcbitmap,
                out IntPtr dstbitmap
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBitmapLockBits(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
                ref Rectangle rect,
                ImageLockMode flags,
                PixelFormat format,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(BitmapData.PinningMarshaller))]
#endif
                BitmapData lockedBitmapData
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBitmapUnlockBits(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(BitmapData.PinningMarshaller))]
#endif
                BitmapData lockedBitmapData
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBitmapGetPixel(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
                int x,
                int y,
                out int argb
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBitmapSetPixel(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
                int x,
                int y,
                int argb
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipBitmapSetResolution(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef bitmap,
                float dpix,
                float dpiy
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipImageGetFrameDimensionsCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipImageGetFrameDimensionsList(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                Guid* dimensionIDs,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateMetafileFromEmf(
                IntPtr hEnhMetafile,
                [MarshalAs(UnmanagedType.Bool)] bool deleteEmf,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateMetafileFromWmf(
                IntPtr hMetafile,
                [MarshalAs(UnmanagedType.Bool)] bool deleteWmf,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(WmfPlaceableFileHeader.PinningMarshaller))]
#endif
                WmfPlaceableFileHeader wmfplacealbeHeader,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipCreateMetafileFromFile(
                string file,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafile(
                IntPtr referenceHdc,
                EmfType emfType,
                IntPtr pframeRect,
                MetafileFrameUnit frameUnit,
                string? description,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafile(
                IntPtr referenceHdc,
                EmfType emfType,
                ref RectangleF frameRect,
                MetafileFrameUnit frameUnit,
                string? description,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafileI(
                IntPtr referenceHdc,
                EmfType emfType,
                ref Rectangle frameRect,
                MetafileFrameUnit frameUnit,
                string? description,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafileFileName(
                string fileName,
                IntPtr referenceHdc,
                EmfType emfType,
                ref RectangleF frameRect,
                MetafileFrameUnit frameUnit,
                string? description,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafileFileName(
                string fileName,
                IntPtr referenceHdc,
                EmfType emfType,
                IntPtr pframeRect,
                MetafileFrameUnit frameUnit,
                string? description,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafileFileNameI(
                string fileName,
                IntPtr referenceHdc,
                EmfType emfType,
                ref Rectangle frameRect,
                MetafileFrameUnit frameUnit,
                string? description,
                out IntPtr metafile
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipPlayMetafileRecord(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                EmfPlusRecordType recordType,
                int flags,
                int dataSize,
                byte[] data
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSaveGraphics(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                out int state
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawArc(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float x,
                float y,
                float width,
                float height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawArcI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int x,
                int y,
                int width,
                int height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawLinesI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawBezier(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float x1,
                float y1,
                float x2,
                float y2,
                float x3,
                float y3,
                float x4,
                float y4
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawEllipse(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float x,
                float y,
                float width,
                float height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawEllipseI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int x,
                int y,
                int width,
                int height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawLine(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float x1,
                float y1,
                float x2,
                float y2
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawLineI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int x1,
                int y1,
                int x2,
                int y2
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawLines(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawPie(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float x,
                float y,
                float width,
                float height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawPieI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int x,
                int y,
                int width,
                int height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawPolygon(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawPolygonI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillEllipse(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float x,
                float y,
                float width,
                float height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillEllipseI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int x,
                int y,
                int width,
                int height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillPolygon(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                PointF* points,
                int count,
                FillMode brushMode
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillPolygonI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                Point* points,
                int count,
                FillMode brushMode
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillRectangle(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float x,
                float y,
                float width,
                float height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillRectangleI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int x,
                int y,
                int width,
                int height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillRectangles(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                RectangleF* rects,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillRectanglesI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                Rectangle* rects,
                int count
            );

            [LibraryImport(
                LibraryName,
                SetLastError = true,
                StringMarshalling = StringMarshalling.Utf16
            )]
            partial internal static int GdipDrawString(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                string textString,
                int length,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                ref RectangleF layoutRect,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef stringFormat,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImageRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int x,
                int y,
                int width,
                int height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGraphicsClear(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int argb
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawClosedCurve(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawClosedCurveI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawClosedCurve2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawClosedCurve2I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawCurve(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawCurveI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawCurve2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawCurve2I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawCurve3(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count,
                int offset,
                int numberOfSegments,
                float tension
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawCurve3I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count,
                int offset,
                int numberOfSegments,
                float tension
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillClosedCurve(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillClosedCurveI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillClosedCurve2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                PointF* points,
                int count,
                float tension,
                FillMode mode
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillClosedCurve2I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                Point* points,
                int count,
                float tension,
                FillMode mode
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillPie(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                float x,
                float y,
                float width,
                float height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillPieI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
                int x,
                int y,
                int width,
                int height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipMeasureString(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                string textString,
                int length,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                ref RectangleF layoutRect,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef stringFormat,
                ref RectangleF boundingBox,
                out int codepointsFitted,
                out int linesFilled
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipMeasureCharacterRanges(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                string textString,
                int length,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef font,
                ref RectangleF layoutRect,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef stringFormat,
                int characterCount,
                IntPtr[] region
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImageI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int x,
                int y
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImage(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                float x,
                float y
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImagePoints(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImagePointsI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImageRectRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int dstx,
                int dsty,
                int dstwidth,
                int dstheight,
                int srcx,
                int srcy,
                int srcwidth,
                int srcheight,
                GraphicsUnit srcunit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageAttributes,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.DrawImageAbortMarshaller))]
#endif
                Graphics.DrawImageAbort? callback,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef callbackdata
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImagePointsRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                PointF* points,
                int count,
                float srcx,
                float srcy,
                float srcwidth,
                float srcheight,
                GraphicsUnit srcunit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageAttributes,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.DrawImageAbortMarshaller))]
#endif
                Graphics.DrawImageAbort? callback,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef callbackdata
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImageRectRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                float dstx,
                float dsty,
                float dstwidth,
                float dstheight,
                float srcx,
                float srcy,
                float srcwidth,
                float srcheight,
                GraphicsUnit srcunit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageAttributes,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.DrawImageAbortMarshaller))]
#endif
                Graphics.DrawImageAbort? callback,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef callbackdata
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImagePointsRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                Point* points,
                int count,
                int srcx,
                int srcy,
                int srcwidth,
                int srcheight,
                GraphicsUnit srcunit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageAttributes,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.DrawImageAbortMarshaller))]
#endif
                Graphics.DrawImageAbort? callback,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef callbackdata
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImageRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                float x,
                float y,
                float width,
                float height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImagePointRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                float x,
                float y,
                float srcx,
                float srcy,
                float srcwidth,
                float srcheight,
                int srcunit
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawImagePointRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int x,
                int y,
                int srcx,
                int srcy,
                int srcwidth,
                int srcheight,
                int srcunit
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawRectangle(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                float x,
                float y,
                float width,
                float height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawRectangleI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                int x,
                int y,
                int width,
                int height
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawRectangles(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                RectangleF* rects,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawRectanglesI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Rectangle* rects,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTransformPoints(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int destSpace,
                int srcSpace,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTransformPointsI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int destSpace,
                int srcSpace,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipLoadImageFromFileICM(string filename, out IntPtr image);

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipLoadImageFromFile(string filename, out IntPtr image);

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetEncoderParameterListSize(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                ref Guid encoder,
                out int size
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetEncoderParameterList(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                ref Guid encoder,
                int size,
                IntPtr buffer
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePath(int brushMode, out IntPtr path);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePath2(
                PointF* points,
                byte* types,
                int count,
                int brushMode,
                out IntPtr path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreatePath2I(
                Point* points,
                byte* types,
                int count,
                int brushMode,
                out IntPtr path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipClonePath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                out IntPtr clonepath
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeletePath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipResetPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPointCount(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                out int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathTypes(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                byte[] types,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathPoints(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathFillMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                out FillMode fillmode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathFillMode(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                FillMode fillmode
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathData(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                GpPathData* pathData
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipStartPathFigure(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipClosePathFigure(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipClosePathFigures(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetPathMarker(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipClearPathMarkers(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipReversePath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathLastPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                out PointF lastPoint
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathLine(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x1,
                float y1,
                float x2,
                float y2
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathLine2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathArc(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x,
                float y,
                float width,
                float height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathBezier(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x1,
                float y1,
                float x2,
                float y2,
                float x3,
                float y3,
                float x4,
                float y4
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathBeziers(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathCurve(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathCurve2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathCurve3(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count,
                int offset,
                int numberOfSegments,
                float tension
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathClosedCurve(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathClosedCurve2(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathRectangle(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x,
                float y,
                float width,
                float height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathRectangles(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                RectangleF* rects,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathEllipse(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x,
                float y,
                float width,
                float height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathPie(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x,
                float y,
                float width,
                float height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathPolygon(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef addingPath,
                [MarshalAs(UnmanagedType.Bool)] bool connect
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipAddPathString(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                string s,
                int length,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontFamily,
                int style,
                float emSize,
                ref RectangleF layoutRect,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipAddPathStringI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                string s,
                int length,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef fontFamily,
                int style,
                float emSize,
                ref Rectangle layoutRect,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef format
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathLineI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x1,
                int y1,
                int x2,
                int y2
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathLine2I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathArcI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x,
                int y,
                int width,
                int height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathBezierI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x1,
                int y1,
                int x2,
                int y2,
                int x3,
                int y3,
                int x4,
                int y4
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathBeziersI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathCurveI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathCurve2I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathCurve3I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count,
                int offset,
                int numberOfSegments,
                float tension
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathClosedCurveI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathClosedCurve2I(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count,
                float tension
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathRectangleI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x,
                int y,
                int width,
                int height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathRectanglesI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Rectangle* rects,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathEllipseI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x,
                int y,
                int width,
                int height
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathPieI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x,
                int y,
                int width,
                int height,
                float startAngle,
                float sweepAngle
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipAddPathPolygonI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipFlattenPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrixfloat,
                float flatness
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipWidenPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                float flatness
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipWarpPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
                PointF* points,
                int count,
                float srcX,
                float srcY,
                float srcWidth,
                float srcHeight,
                WarpMode warpMode,
                float flatness
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipTransformPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetPathWorldBounds(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                out RectangleF gprectf,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef matrix,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisiblePathPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x,
                float y,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsVisiblePathPointI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x,
                int y,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsOutlineVisiblePathPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                float x,
                float y,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipIsOutlineVisiblePathPointI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path,
                int x,
                int y,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                [MarshalAs(UnmanagedType.Bool)] out bool result
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteBrush(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipLoadImageFromStream(IntPtr stream, IntPtr* image);

            [LibraryImport(LibraryName)]
            partial internal static int GdipLoadImageFromStreamICM(IntPtr stream, IntPtr* image);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCloneImage(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out IntPtr cloneimage
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipSaveImageToFile(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                string filename,
                ref Guid classId,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef encoderParams
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSaveImageToStream(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                IntPtr stream,
                Guid* classId,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef encoderParams
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSaveAdd(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef encoderParams
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSaveAddImage(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef newImage,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef encoderParams
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageGraphicsContext(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out IntPtr graphics
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageBounds(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out RectangleF gprectf,
                out GraphicsUnit unit
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImageThumbnail(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                int thumbWidth,
                int thumbHeight,
                out IntPtr thumbImage,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Image.GetThumbnailImageAbortMarshaller))]
#endif
                Image.GetThumbnailImageAbort? callback,
                IntPtr callbackdata
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImagePalette(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                IntPtr palette,
                int size
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipSetImagePalette(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                IntPtr palette
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetImagePaletteSize(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef image,
                out int size
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipImageForceValidation(IntPtr image);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateFromHDC2(
                IntPtr hdc,
                IntPtr hdevice,
                out IntPtr graphics
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateFromHWND(IntPtr hwnd, out IntPtr graphics);

            [LibraryImport(LibraryName)]
            partial internal static int GdipDeleteGraphics(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipReleaseDC(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                IntPtr hdc
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetNearestColor(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                ref int color
            );

            [LibraryImport(LibraryName)]
            partial internal static IntPtr GdipCreateHalftonePalette();

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawBeziers(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                PointF* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipDrawBeziersI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef pen,
                Point* points,
                int count
            );

            [LibraryImport(LibraryName, SetLastError = true)]
            partial internal static int GdipFillPath(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef brush,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef path
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileDestPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref PointF destPoint,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileDestPointI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref Point destPoint,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileDestRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref RectangleF destRect,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileDestRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref Rectangle destRect,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileDestPoints(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                PointF* destPoints,
                int count,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileDestPointsI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                Point* destPoints,
                int count,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileSrcRectDestPoint(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref PointF destPoint,
                ref RectangleF srcRect,
                GraphicsUnit pageUnit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileSrcRectDestPointI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref Point destPoint,
                ref Rectangle srcRect,
                GraphicsUnit pageUnit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileSrcRectDestRect(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref RectangleF destRect,
                ref RectangleF srcRect,
                GraphicsUnit pageUnit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileSrcRectDestRectI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                ref Rectangle destRect,
                ref Rectangle srcRect,
                GraphicsUnit pageUnit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileSrcRectDestPoints(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                PointF* destPoints,
                int count,
                ref RectangleF srcRect,
                GraphicsUnit pageUnit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipEnumerateMetafileSrcRectDestPointsI(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                Point* destPoints,
                int count,
                ref Rectangle srcRect,
                GraphicsUnit pageUnit,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(Graphics.EnumerateMetafileProcMarshaller))]
#endif
                Graphics.EnumerateMetafileProc callback,
                IntPtr callbackdata,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef imageattributes
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipRestoreGraphics(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int state
            );

            [LibraryImport(LibraryName, EntryPoint = "GdipGetMetafileHeaderFromWmf")]
            partial private static int GdipGetMetafileHeaderFromWmf_Internal(
                IntPtr hMetafile,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(WmfPlaceableFileHeader.PinningMarshaller))]
#endif
                WmfPlaceableFileHeader wmfplaceable,
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(MetafileHeaderWmf.Marshaller))]
                    ref MetafileHeaderWmf metafileHeaderWmf
#else
                MetafileHeaderWmf metafileHeaderWmf
#endif
            );

            internal static int GdipGetMetafileHeaderFromWmf(
                IntPtr hMetafile,
                WmfPlaceableFileHeader wmfplaceable,
                MetafileHeaderWmf metafileHeaderWmf
            )
            {
                return GdipGetMetafileHeaderFromWmf_Internal(
                    hMetafile,
                    wmfplaceable,
#if NET7_0_OR_GREATER
                    ref
#endif
                    metafileHeaderWmf
                );
            }

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetMetafileHeaderFromEmf(
                IntPtr hEnhMetafile,
                MetafileHeaderEmf metafileHeaderEmf
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipGetMetafileHeaderFromFile(
                string filename,
                IntPtr header
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetMetafileHeaderFromStream(
                IntPtr stream,
                IntPtr header
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetMetafileHeaderFromMetafile(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                IntPtr header
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipGetHemfFromMetafile(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef metafile,
                out IntPtr hEnhMetafile
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateMetafileFromStream(
                IntPtr stream,
                IntPtr* metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafileStream(
                IntPtr stream,
                IntPtr referenceHdc,
                EmfType emfType,
                RectangleF* frameRect,
                MetafileFrameUnit frameUnit,
                string? description,
                IntPtr* metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafileStream(
                IntPtr stream,
                IntPtr referenceHdc,
                EmfType emfType,
                IntPtr pframeRect,
                MetafileFrameUnit frameUnit,
                string? description,
                IntPtr* metafile
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipRecordMetafileStreamI(
                IntPtr stream,
                IntPtr referenceHdc,
                EmfType emfType,
                Rectangle* frameRect,
                MetafileFrameUnit frameUnit,
                string? description,
                IntPtr* metafile
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipComment(
#if NET7_0_OR_GREATER
                [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
                HandleRef graphics,
                int sizeData,
                byte[] data
            );

            [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
            partial internal static int GdipCreateFontFromLogfontW(
                IntPtr hdc,
                ref Interop.User32.LOGFONT lf,
                out IntPtr font
            );

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateBitmapFromStream(IntPtr stream, IntPtr* bitmap);

            [LibraryImport(LibraryName)]
            partial internal static int GdipCreateBitmapFromStreamICM(
                IntPtr stream,
                IntPtr* bitmap
            );
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StartupInputEx
        {
            public int GdiplusVersion; // Must be 1 or 2

            public IntPtr DebugEventCallback;

            public Interop.BOOL SuppressBackgroundThread; // FALSE unless you're prepared to call

            // the hook/unhook functions properly

            public Interop.BOOL SuppressExternalCodecs; // FALSE unless you want GDI+ only to use

            // its internal image codecs.
            public int StartupParameters;

            public static StartupInputEx GetDefault()
            {
                OperatingSystem os = Environment.OSVersion;
                StartupInputEx result = default;

                // In Windows 7 GDI+1.1 story is different as there are different binaries per GDI+ version.
                bool isWindows7 =
                    os.Platform == PlatformID.Win32NT
                    && os.Version.Major == 6
                    && os.Version.Minor == 1;
                result.GdiplusVersion = isWindows7 ? 1 : 2;
                result.SuppressBackgroundThread = Interop.BOOL.FALSE;
                result.SuppressExternalCodecs = Interop.BOOL.FALSE;
                result.StartupParameters = 0;
                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct StartupOutput
        {
            // The following 2 fields won't be used.  They were originally intended
            // for getting GDI+ to run on our thread - however there are marshalling
            // dealing with function *'s and what not - so we make explicit calls
            // to gdi+ after the fact, via the GdiplusNotificationHook and
            // GdiplusNotificationUnhook methods.
            public IntPtr hook; //not used
            public IntPtr unhook; //not used.
        }
    }
}
