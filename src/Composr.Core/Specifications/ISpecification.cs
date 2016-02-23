using System;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Core.Specifications
{
    /// <summary>
    /// interface to validate any of the core composr elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpecification<T>
    {
        Compliance EvaluateCompliance(T t);
    }

    internal class AndSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> spec1;
        private readonly ISpecification<T> spec2;

        protected ISpecification<T> Spec1
        {
            get
            {
                return spec1;
            }
        }

        protected ISpecification<T> Spec2
        {
            get
            {
                return spec2;
            }
        }

        internal AndSpecification(ISpecification<T> spec1, ISpecification<T> spec2)
        {
            if (spec1 == null) throw new ArgumentNullException("spec1");
            if (spec2 == null) throw new ArgumentNullException("spec2");

            this.spec1 = spec1;
            this.spec2 = spec2;
        }

        public Compliance EvaluateCompliance(T candidate)
        {
            return Spec1.EvaluateCompliance(candidate) & Spec2.EvaluateCompliance(candidate);
        }
    }

    internal class OrSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> spec1;
        private readonly ISpecification<T> spec2;

        protected ISpecification<T> Spec1
        {
            get
            {
                return spec1;
            }
        }

        protected ISpecification<T> Spec2
        {
            get
            {
                return spec2;
            }
        }

        internal OrSpecification(ISpecification<T> spec1, ISpecification<T> spec2)
        {
            if (spec1 == null) throw new ArgumentNullException("spec1");
            if (spec2 == null) throw new ArgumentNullException("spec2");

            this.spec1 = spec1;
            this.spec2 = spec2;
        }

        public Compliance EvaluateCompliance(T candidate)
        {
            return Spec1.EvaluateCompliance(candidate) | Spec2.EvaluateCompliance(candidate);
        }
    }

    internal class NotSpecification<T> : ISpecification<T>
    {
        private readonly ISpecification<T> spec;

        protected ISpecification<T> Specification
        {
            get
            {
                return spec;
            }
        }

        internal NotSpecification(ISpecification<T> spec)
        {
            if (spec == null)
            {
                throw new ArgumentNullException("spec");
            }

            this.spec = spec;
        }

        public Compliance EvaluateCompliance(T candidate)
        {
            return !spec.EvaluateCompliance(candidate);
        }
    }

    public static class SpecificationExtensionMethods
    {
        public static ISpecification<T> And<T>(this ISpecification<T> spec1, ISpecification<T> spec2)
        {
            return new AndSpecification<T>(spec1, spec2);
        }

        public static ISpecification<T> Or<T>(this ISpecification<T> spec1, ISpecification<T> spec2)
        {
            return new OrSpecification<T>(spec1, spec2);
        }

        public static ISpecification<T> Not<T>(this ISpecification<T> spec)
        {
            return new NotSpecification<T>(spec);
        }
    }

    public class Compliance
    {
        public Compliance()
        {
            Initialize();
        }

        public Compliance(IEnumerable<string> errors)
        {
            Initialize();
            Errors.AddRange(errors.Where(failure => !string.IsNullOrWhiteSpace(failure)));
        }

        private void Initialize()
        {
            Errors = new List<string>();
        }

        public bool IsSatisfied
        {
            get{return Errors.Count == 0;}
        }

        public List<string> Errors { get; private set; }

        public static Compliance operator &(Compliance c1, Compliance c2)
        {
            if (c1 == null || c2 == null) throw new ArgumentNullException();
            return new Compliance(c1.Errors.Concat(c2.Errors));
        }

        public static Compliance operator |(Compliance c1, Compliance c2)
        {
            if (c1 == null || c2 == null) throw new ArgumentNullException();
            return c1.IsSatisfied || c2.IsSatisfied ? new Compliance() : new Compliance(c1.Errors.Concat(c2.Errors));
        }

        public static Compliance operator !(Compliance c)
        {
            if (c == null) throw new ArgumentNullException();
            return !c.IsSatisfied ? new Compliance() : new Compliance(c.Errors);
        }
    }
}
