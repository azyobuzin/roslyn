// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeStyle;
using Microsoft.CodeAnalysis.CSharp.CodeStyle;
using Microsoft.CodeAnalysis.CSharp.ImplementAbstractClass;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Diagnostics;
using Microsoft.CodeAnalysis.ImplementType;
using Microsoft.CodeAnalysis.Options;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.ImplementAbstractClass
{
    public partial class ImplementAbstractClassTests : AbstractCSharpDiagnosticProviderBasedUserDiagnosticTest
    {
        internal override (DiagnosticAnalyzer, CodeFixProvider) CreateDiagnosticProviderAndFixer(Workspace workspace)
            => (null, new CSharpImplementAbstractClassCodeFixProvider());

        private IDictionary<OptionKey, object> AllOptionsOff =>
            OptionsSet(
                 SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.NeverWithNoneEnforcement),
                 SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedConstructors, CSharpCodeStyleOptions.NeverWithNoneEnforcement),
                 SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedOperators, CSharpCodeStyleOptions.NeverWithNoneEnforcement),
                 SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, CSharpCodeStyleOptions.NeverWithNoneEnforcement),
                 SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, CSharpCodeStyleOptions.NeverWithNoneEnforcement),
                 SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedIndexers, CSharpCodeStyleOptions.NeverWithNoneEnforcement));

        internal Task TestAllOptionsOffAsync(
            string initialMarkup, string expectedMarkup,
            int index = 0, bool ignoreTrivia = true,
            IDictionary<OptionKey, object> options = null)
        {
            options = options ?? new Dictionary<OptionKey, object>();
            foreach (var kvp in AllOptionsOff)
            {
                options.Add(kvp);
            }

            return TestInRegularAndScriptAsync(
                initialMarkup, expectedMarkup,
                index: index, ignoreTrivia: ignoreTrivia, options: options);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestSimpleMethods()
        {
            await TestAllOptionsOffAsync(
@"abstract class Foo
{
    protected abstract string FooMethod();
    public abstract void Blah();
}

abstract class Bar : Foo
{
    public abstract bool BarMethod();

    public override void Blah()
    {
    }
}

class [|Program|] : Foo
{
    static void Main(string[] args)
    {
    }
}",
@"abstract class Foo
{
    protected abstract string FooMethod();
    public abstract void Blah();
}

abstract class Bar : Foo
{
    public abstract bool BarMethod();

    public override void Blah()
    {
    }
}

class Program : Foo
{
    static void Main(string[] args)
    {
    }

    public override void Blah()
    {
        throw new System.NotImplementedException();
    }

    protected override string FooMethod()
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        [WorkItem(16434, "https://github.com/dotnet/roslyn/issues/16434")]
        public async Task TestMethodWithTupleNames()
        {
            await TestAllOptionsOffAsync(
@"abstract class Base
{
    protected abstract (int a, int b) Method((string, string d) x);
}

class [|Program|] : Base
{
}",
@"abstract class Base
{
    protected abstract (int a, int b) Method((string, string d) x);
}

class Program : Base
{
    protected override (int a, int b) Method((string, string d) x)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(543234, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/543234")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestNotAvailableForStruct()
        {
            await TestMissingInRegularAndScriptAsync(
@"abstract class Foo
{
    public abstract void Bar();
}

struct [|Program|] : Foo
{
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalIntParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(int x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(int x = 3);
}

class b : d
{
    public override void foo(int x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalCharParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(char x = 'a');
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(char x = 'a');
}

class b : d
{
    public override void foo(char x = 'a')
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalStringParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(string x = ""x"");
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(string x = ""x"");
}

class b : d
{
    public override void foo(string x = ""x"")
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalShortParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(short x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(short x = 3);
}

class b : d
{
    public override void foo(short x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalDecimalParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(decimal x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(decimal x = 3);
}

class b : d
{
    public override void foo(decimal x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalDoubleParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(double x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(double x = 3);
}

class b : d
{
    public override void foo(double x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalLongParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(long x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(long x = 3);
}

class b : d
{
    public override void foo(long x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalFloatParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(float x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(float x = 3);
}

class b : d
{
    public override void foo(float x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalUshortParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(ushort x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(ushort x = 3);
}

class b : d
{
    public override void foo(ushort x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalUintParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(uint x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(uint x = 3);
}

class b : d
{
    public override void foo(uint x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalUlongParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void foo(ulong x = 3);
}

class [|b|] : d
{
}",
@"abstract class d
{
    public abstract void foo(ulong x = 3);
}

class b : d
{
    public override void foo(ulong x = 3)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalStructParameter()
        {
            await TestAllOptionsOffAsync(
@"struct b
{
}

abstract class d
{
    public abstract void foo(b x = new b());
}

class [|c|] : d
{
}",
@"struct b
{
}

abstract class d
{
    public abstract void foo(b x = new b());
}

class c : d
{
    public override void foo(b x = default(b))
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(916114, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/916114")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalNullableStructParameter()
        {
            await TestAllOptionsOffAsync(
@"struct b
{
}

abstract class d
{
    public abstract void m(b? x = null, b? y = default(b?));
}

class [|c|] : d
{
}",
@"struct b
{
}

abstract class d
{
    public abstract void m(b? x = null, b? y = default(b?));
}

class c : d
{
    public override void m(b? x = null, b? y = null)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(916114, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/916114")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalNullableIntParameter()
        {
            await TestAllOptionsOffAsync(
@"abstract class d
{
    public abstract void m(int? x = 5, int? y = default(int?));
}

class [|c|] : d
{
}",
@"abstract class d
{
    public abstract void m(int? x = 5, int? y = default(int?));
}

class c : d
{
    public override void m(int? x = 5, int? y = null)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOptionalObjectParameter()
        {
            await TestAllOptionsOffAsync(
@"class b
{
}

abstract class d
{
    public abstract void foo(b x = null);
}

class [|c|] : d
{
}",
@"class b
{
}

abstract class d
{
    public abstract void foo(b x = null);
}

class c : d
{
    public override void foo(b x = null)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(543883, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/543883")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestDifferentAccessorAccessibility()
        {
            await TestAllOptionsOffAsync(
@"abstract class c1
{
    public abstract c1 this[c1 x] { get; internal set; }
}

class [|c2|] : c1
{
}",
@"abstract class c1
{
    public abstract c1 this[c1 x] { get; internal set; }
}

class c2 : c1
{
    public override c1 this[c1 x]
    {
        get
        {
            throw new System.NotImplementedException();
        }

        internal set
        {
            throw new System.NotImplementedException();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestEvent1()
        {
            await TestAllOptionsOffAsync(
@"using System;

abstract class C
{
    public abstract event Action E;
}

class [|D|] : C
{
}",
@"using System;

abstract class C
{
    public abstract event Action E;
}

class D : C
{
    public override event Action E;
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestIndexer1()
        {
            await TestAllOptionsOffAsync(
@"using System;

abstract class C
{
    public abstract int this[string s]
    {
        get
        {
        }

        internal set
        {
        }
    }
}

class [|D|] : C
{
}",
@"using System;

abstract class C
{
    public abstract int this[string s]
    {
        get
        {
        }

        internal set
        {
        }
    }
}

class D : C
{
    public override int this[string s]
    {
        get
        {
            throw new NotImplementedException();
        }

        internal set
        {
            throw new NotImplementedException();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestMissingInHiddenType()
        {
            await TestMissingInRegularAndScriptAsync(
@"using System;

abstract class Foo
{
    public abstract void F();
}

class [|Program|] : Foo
{
#line hidden
}
#line default");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestGenerateIntoNonHiddenPart()
        {
            await TestAllOptionsOffAsync(
@"using System;

abstract class Foo { public abstract void F(); }

partial class [|Program|] : Foo
{
#line hidden
}
#line default

partial class Program ",
@"using System;

abstract class Foo { public abstract void F(); }

partial class Program : Foo
{
#line hidden
}
#line default

partial class Program
{
    public override void F()
    {
        throw new NotImplementedException();
    }
}
",
ignoreTrivia: false);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestGenerateIfLocationAvailable()
        {
            await TestAllOptionsOffAsync(
@"#line default
using System;

abstract class Foo { public abstract void F(); }

partial class [|Program|] : Foo
{
    void Bar()
    {
    }

#line hidden
}
#line default",
@"#line default
using System;

abstract class Foo { public abstract void F(); }

partial class Program : Foo
{
    public override void F()
    {
        throw new NotImplementedException();
    }

    void Bar()
    {
    }

#line hidden
}
#line default",
ignoreTrivia: false);
        }

        [WorkItem(545585, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545585")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestOnlyGenerateUnimplementedAccessors()
        {
            await TestAllOptionsOffAsync(
@"using System;

abstract class A
{
    public abstract int X { get; set; }
}

abstract class B : A
{
    public override int X
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}

class [|C|] : B
{
}",
@"using System;

abstract class A
{
    public abstract int X { get; set; }
}

abstract class B : A
{
    public override int X
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}

class C : B
{
    public override int X
    {
        set
        {
            throw new NotImplementedException();
        }
    }
}");
        }

        [WorkItem(545615, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545615")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestParamsArray()
        {
            await TestAllOptionsOffAsync(
@"class A
{
    public virtual void Foo(int x, params int[] y)
    {
    }
}

abstract class B : A
{
    public abstract override void Foo(int x, int[] y = null);
}

class [|C|] : B
{
}",
@"class A
{
    public virtual void Foo(int x, params int[] y)
    {
    }
}

abstract class B : A
{
    public abstract override void Foo(int x, int[] y = null);
}

class C : B
{
    public override void Foo(int x, params int[] y)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(545636, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545636")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestNullPointerType()
        {
            await TestAllOptionsOffAsync(
@"abstract class C
{
    unsafe public abstract void Foo(int* x = null);
}

class [|D|] : C
{
}",
@"abstract class C
{
    unsafe public abstract void Foo(int* x = null);
}

class D : C
{
    public override unsafe void Foo(int* x = null)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(545637, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545637")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestErrorTypeCalledVar()
        {
            await TestAllOptionsOffAsync(
@"extern alias var;

abstract class C
{
    public abstract void Foo(var::X x);
}

class [|D|] : C
{
}",
@"extern alias var;

abstract class C
{
    public abstract void Foo(var::X x);
}

class D : C
{
    public override void Foo(X x)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task Bugfix_581500()
        {
            await TestAllOptionsOffAsync(
@"abstract class A<T>
{
    public abstract void M(T x);

    abstract class B : A<B>
    {
        class [|T|] : A<T>
        {
        }
    }
}",
@"abstract class A<T>
{
    public abstract void M(T x);

    abstract class B : A<B>
    {
        class T : A<T>
        {
            public override void M(B.T x)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}");
        }

        [WorkItem(625442, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/625442")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task Bugfix_625442()
        {
            await TestAllOptionsOffAsync(
@"abstract class A<T>
{
    public abstract void M(T x);
    abstract class B : A<B>
    {
        class [|T|] : A<B.T> { }
    }
}
",
@"abstract class A<T>
{
    public abstract void M(T x);
    abstract class B : A<B>
    {
        class T : A<B.T>
        {
            public override void M(A<A<T>.B>.B.T x)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
", ignoreTrivia: false);
        }

        [WorkItem(2407, "https://github.com/dotnet/roslyn/issues/2407")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task ImplementClassWithInaccessibleMembers()
        {
            await TestAllOptionsOffAsync(
@"using System;
using System.Globalization;

public class [|x|] : EastAsianLunisolarCalendar
{
}",
@"using System;
using System.Globalization;

public class x : EastAsianLunisolarCalendar
{
    public override int[] Eras
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    internal override int MinCalendarYear
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    internal override int MaxCalendarYear
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    internal override EraInfo[] CalEraInfo
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    internal override DateTime MinDate
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    internal override DateTime MaxDate
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override int GetEra(DateTime time)
    {
        throw new NotImplementedException();
    }

    internal override int GetGregorianYear(int year, int era)
    {
        throw new NotImplementedException();
    }

    internal override int GetYear(int year, DateTime time)
    {
        throw new NotImplementedException();
    }

    internal override int GetYearInfo(int LunarYear, int Index)
    {
        throw new NotImplementedException();
    }
}");
        }

        [WorkItem(13149, "https://github.com/dotnet/roslyn/issues/13149")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestPartialClass1()
        {
            await TestAllOptionsOffAsync(
@"using System;

public abstract class Base
{
    public abstract void Dispose();
}

partial class [|A|] : Base
{
}

partial class A
{
}",
@"using System;

public abstract class Base
{
    public abstract void Dispose();
}

partial class A : Base
{
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}

partial class A
{
}");
        }

        [WorkItem(13149, "https://github.com/dotnet/roslyn/issues/13149")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestPartialClass2()
        {
            await TestAllOptionsOffAsync(
@"using System;

public abstract class Base
{
    public abstract void Dispose();
}

partial class [|A|]
{
}

partial class A : Base
{
}",
@"using System;

public abstract class Base
{
    public abstract void Dispose();
}

partial class A
{
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}

partial class A : Base
{
}");
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Method1()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract void M(int x);
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract void M(int x);
}

class T : A
{
    public override void M(int x) => throw new System.NotImplementedException();
}", options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.WhenPossibleWithNoneEnforcement));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Property1()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int M { get; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int M { get; }
}

class T : A
{
    public override int M => throw new System.NotImplementedException();
}", options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, CSharpCodeStyleOptions.WhenPossibleWithNoneEnforcement));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Property3()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int M { set; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int M { set; }
}

class T : A
{
    public override int M
    {
        set
        {
            throw new System.NotImplementedException();
        }
    }
}", options: OptionsSet(
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, ExpressionBodyPreference.WhenPossible, NotificationOption.None),
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, ExpressionBodyPreference.Never, NotificationOption.None)));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Property4()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int M { get; set; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int M { get; set; }
}

class T : A
{
    public override int M
    {
        get
        {
            throw new System.NotImplementedException();
        }

        set
        {
            throw new System.NotImplementedException();
        }
    }
}", options: OptionsSet(
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, ExpressionBodyPreference.WhenPossible, NotificationOption.None),
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, ExpressionBodyPreference.Never, NotificationOption.None)));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Indexers1()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int this[int i] { get; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int this[int i] { get; }
}

class T : A
{
    public override int this[int i] => throw new System.NotImplementedException();
}", options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedIndexers, CSharpCodeStyleOptions.WhenPossibleWithNoneEnforcement));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Indexer3()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int this[int i] { set; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int this[int i] { set; }
}

class T : A
{
    public override int this[int i]
    {
        set
        {
            throw new System.NotImplementedException();
        }
    }
}", options: OptionsSet(
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedIndexers, ExpressionBodyPreference.WhenPossible, NotificationOption.None),
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, ExpressionBodyPreference.Never, NotificationOption.None)));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Indexer4()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int this[int i] { get; set; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int this[int i] { get; set; }
}

class T : A
{
    public override int this[int i]
    {
        get
        {
            throw new System.NotImplementedException();
        }

        set
        {
            throw new System.NotImplementedException();
        }
    }
}", options: OptionsSet(
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedIndexers, ExpressionBodyPreference.WhenPossible, NotificationOption.None),
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, ExpressionBodyPreference.Never, NotificationOption.None)));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Accessor1()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int M { get; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int M { get; }
}

class T : A
{
    public override int M
    {
        get => throw new System.NotImplementedException();
        }
    }", options: OptionsSet(
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedProperties, ExpressionBodyPreference.Never, NotificationOption.None),
    SingleOption(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, ExpressionBodyPreference.WhenPossible, NotificationOption.None)));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Accessor3()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int M { set; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int M { set; }
}

class T : A
{
    public override int M
    {
        set => throw new System.NotImplementedException();
        }
    }", options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, CSharpCodeStyleOptions.WhenPossibleWithNoneEnforcement));
        }

        [WorkItem(581500, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/581500")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestCodeStyle_Accessor4()
        {
            await TestInRegularAndScriptAsync(
@"abstract class A
{
    public abstract int M { get; set; }
}

class [|T|] : A
{
}",
@"abstract class A
{
    public abstract int M { get; set; }
}

class T : A
{
    public override int M
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
        }
    }", options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedAccessors, CSharpCodeStyleOptions.WhenPossibleWithNoneEnforcement));
        }

        [WorkItem(15387, "https://github.com/dotnet/roslyn/issues/15387")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestWithGroupingOff1()
        {
            await TestInRegularAndScriptAsync(
@"abstract class Base
{
    public abstract int Prop { get; }
}

class [|Derived|] : Base
{
    void Foo() { }
}",
@"abstract class Base
{
    public abstract int Prop { get; }
}

class Derived : Base
{
    void Foo() { }

    public override int Prop => throw new System.NotImplementedException();
}", options: Option(ImplementTypeOptions.InsertionBehavior, ImplementTypeInsertionBehavior.AtTheEnd));
        }

        [WorkItem(17274, "https://github.com/dotnet/roslyn/issues/17274")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestAddedUsingWithBanner1()
        {
            await TestInRegularAndScriptAsync(
@"// Copyright ...

using Microsoft.Win32;

namespace My
{
    public abstract class Foo
    {
        public abstract void Bar(System.Collections.Generic.List<object> values);
    }

    public class [|Foo2|] : Foo // Implement Abstract Class
    {
    }
}",
@"// Copyright ...

using System.Collections.Generic;
using Microsoft.Win32;

namespace My
{
    public abstract class Foo
    {
        public abstract void Bar(System.Collections.Generic.List<object> values);
    }

    public class Foo2 : Foo // Implement Abstract Class
    {
        public override void Bar(List<object> values)
        {
            throw new System.NotImplementedException();
        }
    }
}", ignoreTrivia: false);
        }

        [WorkItem(17562, "https://github.com/dotnet/roslyn/issues/17562")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementAbstractClass)]
        public async Task TestNullableOptionalParameters()
        {
            await TestInRegularAndScriptAsync(
@"struct V { }
abstract class B
{
    public abstract void M1(int i = 0, string s = null, int? j = null, V v = default(V));
    public abstract void M2<T>(T? i = null) where T : struct;
}
sealed class [|D|] : B
{
}",
@"struct V { }
abstract class B
{
    public abstract void M1(int i = 0, string s = null, int? j = null, V v = default(V));
    public abstract void M2<T>(T? i = null) where T : struct;
}
sealed class D : B
{
    public override void M1(int i = 0, string s = null, int? j = null, V v = default(V))
    {
        throw new System.NotImplementedException();
    }

    public override void M2<T>(T? i = null)
    {
        throw new System.NotImplementedException();
    }
}");
        }

        [WorkItem(13932, "https://github.com/dotnet/roslyn/issues/13932")]
        [WorkItem(5898, "https://github.com/dotnet/roslyn/issues/5898")]
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsImplementInterface)]
        public async Task TestAutoProperties()
        {
            await TestInRegularAndScript1Async(
@"abstract class AbstractClass
{
    public abstract int ReadOnlyProp { get; }
    public abstract int ReadWriteProp { get; set; }
    public abstract int WriteOnlyProp { set; }
}

class [|C|] : AbstractClass
{
}",
@"abstract class AbstractClass
{
    public abstract int ReadOnlyProp { get; }
    public abstract int ReadWriteProp { get; set; }
    public abstract int WriteOnlyProp { set; }
}

class C : AbstractClass
{
    public override int ReadOnlyProp { get; }
    public override int ReadWriteProp { get; set; }
    public override int WriteOnlyProp { set => throw new System.NotImplementedException(); }
}", parameters: new TestParameters(options: Option(
    ImplementTypeOptions.PropertyGenerationBehavior,
    ImplementTypePropertyGenerationBehavior.PreferAutoProperties)));
        }
    }
}