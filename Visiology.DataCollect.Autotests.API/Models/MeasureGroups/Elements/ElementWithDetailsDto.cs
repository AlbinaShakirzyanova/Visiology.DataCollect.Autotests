﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
{
    /// <summary>
    /// Элемент группы показателей
    /// </summary>
    public class ElementWithDetailsDto : ElementBaseDto, IEquatable<ElementWithDetailsDto>
    {
        /// <summary>
        /// Координаты элемента по измерения
        /// </summary>
        public List<DimensionElementWithDetailsDto> DimensionElements { get; set; }
            = new List<DimensionElementWithDetailsDto>();

        /// <summary>
        /// Координаты элемента по показателями
        /// </summary>
        public List<MeasureElementWithDetailsDto> MeasureElements { get; set; }
            = new List<MeasureElementWithDetailsDto>();

        /// <summary>
        /// Координата по дате
        /// </summary>
        public CalendarDto Calendar { get; set; }

        /// <summary>
        /// Значения атрибутов элемента группы показателей
        /// </summary>
        public List<MeasureGroupElementAttributeDto> Attributes { get; set; }
            = new List<MeasureGroupElementAttributeDto>();

        public override bool Equals(object obj)
        {
            // Осознанно убрано сравнение по идентификатору
            var dto = obj as ElementWithDetailsDto;
            return dto != null &&
                   DimensionElements.SequenceEqual(dto.DimensionElements) &&
                   MeasureElements.SequenceEqual(dto.MeasureElements) &&
                   Calendar.Equals(dto.Calendar) &&
                   Attributes.SequenceEqual(dto.Attributes) &&
                   Value == dto.Value &&
                   Comment == dto.Comment &&
                   SystemInfo == dto.SystemInfo;
        }

        public bool Equals([AllowNull] ElementWithDetailsDto other)
        {
            // Осознанно убрано сравнение по идентификатору
            var dto = other as ElementWithDetailsDto;
            return dto != null &&
                   DimensionElements.SequenceEqual(dto.DimensionElements) &&
                   MeasureElements.SequenceEqual(dto.MeasureElements) &&
                   Calendar.Equals(dto.Calendar) &&
                   Attributes.SequenceEqual(dto.Attributes) &&
                   Value == dto.Value &&
                   Comment == dto.Comment &&
                   SystemInfo == dto.SystemInfo;
        }        

        public override int GetHashCode()
        {
            var hashCode = -2007856361;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<DimensionElementWithDetailsDto>>.Default.GetHashCode(DimensionElements);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<MeasureElementWithDetailsDto>>.Default.GetHashCode(MeasureElements);
            hashCode = hashCode * -1521134295 + EqualityComparer<CalendarDto>.Default.GetHashCode(Calendar);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<MeasureGroupElementAttributeDto>>.Default.GetHashCode(Attributes);
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SystemInfo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Comment);
            return hashCode;
        }
    }
}
